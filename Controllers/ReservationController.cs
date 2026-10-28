namespace Restaurant.Controllers
{
    public class ReservationController:Controller
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public ReservationController(ILogger<ReservationController> logger, UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager, IUnitOfWork context, IMapper mapper)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Uitzondering: Niet ingelogd -> Start UseCase Inloggen
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new ReservationViewModel
            {
                // Haal de tijdsloten op uit de database voor de dropdown
                BeschikbareTijdSloten = await _context.CustomTafelRepository.GetTijdSlotSelectListAsync()
            };

            return View(model);
        }

        // POST: /Reservation/Index (Verwerkt het formulier)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ReservationViewModel model)
        {
            // 1. Check of gebruiker nog ingelogd is
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Login", "Account");
            }

            // Zorg dat de dropdown opnieuw wordt geladen als we straks de view terugsturen bij een fout
            model.BeschikbareTijdSloten = await _context.CustomTafelRepository.GetTijdSlotSelectListAsync();

            // 2. Uitzondering: Datum in het verleden
            if (model.Date.Date < DateTime.Today)
            {
                ModelState.AddModelError("Date", "De gekozen datum kan niet in het verleden liggen.");
                return View(model);
            }

            // 3. Controleer of er een tafel beschikbaar is via de TafelRepository
            bool isAvailable = await _context.CustomTafelRepository.CheckAvailabilityAsync(model.Date, model.TijdSlotId, model.Persons);

            if (!isAvailable)
            {
                // Uitzondering: Geen beschikbare tafels -> Toon alternatieve tijdstippen
                ModelState.AddModelError(string.Empty, "Geen tafels beschikbaar voor dit tijdslot op deze datum.");
                model.AlternativeSlots = await _context.CustomTafelRepository.GetAlternativeSlotsAsync(model.Date, model.Persons);
                return View(model);
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);

                // Gebruik AutoMapper om het ViewModel te mappen naar jouw Reservatie model
                var reservatie = _mapper.Map<Reservatie>(model);

                // Velden instellen die niet via het formulier komen
                reservatie.KlantId = currentUser.Id;
                reservatie.Betaald = false;
                reservatie.IsAanwezig = false;

                // 4. Opslaan in de database via UnitOfWork
                await _context.ReservatieRepository.AddAsync(reservatie);
                await _context.SaveChangesAsync();

                // 5. Verstuur bevestigingsmail (Definition of Done)
                // Hier kun je jouw IEmailSender service aanroepen als je die hebt geconfigureerd:
                // await _emailSender.SendEmailAsync(currentUser.Email, "Bevestiging Reservatie", "Uw reservatie is succesvol opgeslagen.");

                _logger.LogInformation($"Reservatie succesvol opgeslagen voor gebruiker {currentUser.Email}");

                // Redirect naar het klantprofiel waar de reservatie zichtbaar is (Definition of Done)
                return RedirectToAction("Profile", "Account", new { success = "reservation_created" });
            }
            catch (Exception ex)
            {
                // Uitzondering: Systeemfout bij opslaan -> Toon foutmelding en behoud ingevoerde data
                _logger.LogError(ex, "Systeemfout bij het opslaan van een reservatie.");
                ModelState.AddModelError(string.Empty, "Er is een systeemfout opgetreden bij het opslaan. Probeer het opnieuw.");
                return View(model);
            }
        }
    }
}
