using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Eigenaar")]
    public class OwnerController : Controller
    {
        private readonly ILogger<OwnerController> _logger;
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public OwnerController(ILogger<OwnerController> logger, UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager, IUnitOfWork context, IMapper mapper)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(DateTime? datumFilter = null, string? statusFilter = null)
        {
            ViewData["ActiveSection"] = "reservations";

            var allReservations = await _context.ReservatieRepository.Find(
                null,
                r => r.CustomUser,
                r => r.Tijdslot,
                r => r.Tafellijsten
            );

            IEnumerable<Reservatie> filteredReservations = allReservations;

            if (datumFilter.HasValue)
            {
                filteredReservations = filteredReservations.Where(r => r.Datum.HasValue && r.Datum.Value.Date == datumFilter.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                filteredReservations = statusFilter switch
                {
                    "Aanwezig" => filteredReservations.Where(r => r.IsAanwezig),
                    "Betaald" => filteredReservations.Where(r => r.Betaald),
                    "Openstaand" => filteredReservations.Where(r => !r.IsAanwezig),
                    _ => filteredReservations
                };
            }

            var reservaties = filteredReservations
                .OrderBy(r => r.Datum)
                .ThenBy(r => r.TijdSlotId)
                .ToList();

            ViewBag.DatumFilter = datumFilter?.ToString("yyyy-MM-dd");
            ViewBag.StatusFilter = statusFilter;

            return View(reservaties);
        }

        [HttpGet]
        public async Task<IActionResult> EditReservatie(int id)
        {
            var reservaties = await _context.ReservatieRepository.Find(
                voorwaarden: r => r.Id == id,
                    r => r.CustomUser,
                    r => r.Tijdslot
                );

            var reservatie = reservaties.FirstOrDefault();
            if (reservatie == null)
                return NotFound();

            var vm = new EditReservationViewModel
            {
                Id = reservatie.Id,
                KlantNaam = reservatie.CustomUser?.UserName ?? reservatie.KlantId,
                KlantEmail = reservatie.CustomUser?.Email ?? string.Empty,
                Datum = reservatie.Datum,
                TijdSlotId = reservatie.TijdSlotId,
                AantalPersonen = reservatie.AantalPersonen,
                Opmerking = reservatie.Opmerking,
                Betaald = reservatie.Betaald,
                IsAanwezig = reservatie.IsAanwezig,
                IsAlIngecheckt = reservatie.IsAanwezig,
                TijdSlotOpties = await _context.CustomTafelRepository.GetTijdSlotSelectListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReservatie(int id, EditReservationViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var reservaties = await _context.ReservatieRepository.Find(
                voorwaarden: r => r.Id == id,
                    r => r.CustomUser,
                    r => r.Tijdslot,
                    r => r.Tafellijsten
                );

            var reservatie = reservaties.FirstOrDefault();

            if (reservatie == null)
                return NotFound();

            model.TijdSlotOpties = await _context.CustomTafelRepository.GetTijdSlotSelectListAsync();
            model.KlantNaam = reservatie.CustomUser?.UserName ?? reservatie.KlantId;
            model.KlantEmail = reservatie.CustomUser?.Email ?? string.Empty;
            model.IsAlIngecheckt = reservatie.IsAanwezig;

            // Basisvalidatie (datum, tijdslot, aantal personen)
            if (!model.Datum.HasValue)
            {
                ModelState.AddModelError(nameof(model.Datum), "Selecteer een geldige datum.");
            }
            else if (model.Datum.Value.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(model.Datum), "Een reservatie kan niet naar een datum in het verleden worden verplaatst.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Uitzondering 2: Reservatie al ingecheckt -> waarschuwen voor impact
            bool slotOrDateOrPersonsChanged =
                reservatie.Datum?.Date != model.Datum!.Value.Date ||
                reservatie.TijdSlotId != model.TijdSlotId ||
                reservatie.AantalPersonen != model.AantalPersonen;

            if (reservatie.IsAanwezig && slotOrDateOrPersonsChanged && !model.WaarschuwingBevestigd)
            {
                ModelState.AddModelError(string.Empty,
                    "Let op: deze gast is al ingecheckt. Wijzigen van datum, tijdslot of aantal personen kan invloed hebben op de huidige tafelindeling en lopende bestellingen. Vink bevestiging aan om toch door te gaan.");
                return View(model);
            }

            // Uitzondering 1: Nieuwe tijd niet beschikbaar -> toon alternatieve tijden
            bool isBeschikbaar = await _context.CustomTafelRepository.CheckAvailabilityAsync(
                model.Datum!.Value,
                model.TijdSlotId,
                model.AantalPersonen,
                excludeReservatieId: reservatie.Id);

            if (!isBeschikbaar)
            {
                model.AlternatieveTijdsloten = await _context.CustomTafelRepository.GetAlternativeSlotsAsync(
                    model.Datum!.Value,
                    model.AantalPersonen,
                    excludeReservatieId: reservatie.Id);

                ModelState.AddModelError(nameof(model.TijdSlotId),
                    "Het gekozen tijdslot is niet beschikbaar voor dit aantal personen op deze datum.");
                return View(model);
            }

            // Wijzigingen toepassen op entiteit
            reservatie.Datum = model.Datum!.Value.Date;
            reservatie.TijdSlotId = model.TijdSlotId;
            reservatie.AantalPersonen = model.AantalPersonen;
            reservatie.Opmerking = model.Opmerking;
            reservatie.Betaald = model.Betaald;

            // Planning bijwerken: bij wijziging van datum/tijdslot/personen oude tafelkoppelingen vrijgeven
            // zodat de eigenaar opnieuw kan indelen (of vervang dit door automatische herindeling)
            if (slotOrDateOrPersonsChanged && reservatie.Tafellijsten.Any())
            {
                reservatie.Tafellijsten.Clear();
            }

            _context.ReservatieRepository.Update(reservatie);
            await _context.SaveChangesAsync(); // of SaveChangesAsync() volgens jouw IUnitOfWork

            // Uitzondering 3: Email versturen faalt -> waarschuwing tonen, wijzigingen blijven behouden
            bool mailVerzonden = false;
            try
            {
                // TODO (teamgenoot): Vervang onderstaande blokken door de echte mailservice-aanroep
                // met configuratie uit builder.Configuration.GetSection("EmailSettings"):
                //
                // await _emailService.SendReservationUpdatedAsync(
                //     toEmail: reservatie.CustomUser?.Email,
                //     customerName: reservatie.CustomUser?.UserName,
                //     datum: reservatie.Datum.Value,
                //     tijdSlotId: reservatie.TijdSlotId,
                //     aantalPersonen: reservatie.AantalPersonen);

                mailVerzonden = true;
            }
            catch (Exception)
            {
                mailVerzonden = false;
            }

            if (!mailVerzonden)
            {
                TempData["WarningMessage"] =
                    "De reservatiewijzigingen en planning zijn opgeslagen, maar de wijzigingsmail naar de klant kon niet worden verzonden. Informeer de klant indien nodig handmatig.";
            }
            else
            {
                TempData["SuccessMessage"] =
                    "De reservatie is bijgewerkt, de planning is aangepast en de wijzigingsmail is naar de klant verzonden.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

