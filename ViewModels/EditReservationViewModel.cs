namespace Restaurant.ViewModels
{
    public class EditReservationViewModel
    {
        public int Id { get; set; }

        public string KlantNaam { get; set; } = string.Empty;
        public string KlantEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Datum is verplicht.")]
        [DataType(DataType.Date)]
        public DateTime? Datum { get; set; }

        [Required(ErrorMessage = "Tijdslot is verplicht.")]
        public int TijdSlotId { get; set; }

        [Required(ErrorMessage = "Aantal personen is verplicht.")]
        [Range(1, 50, ErrorMessage = "Aantal personen moet tussen 1 en 50 liggen.")]
        public int AantalPersonen { get; set; }

        public string? Opmerking { get; set; }

        public bool Betaald { get; set; }
        public bool IsAanwezig { get; set; }

        // Exception handling state
        public bool IsAlIngecheckt { get; set; }
        public bool WaarschuwingBevestigd { get; set; }
        public List<string> AlternatieveTijdsloten { get; set; } = new();

        // Dropdown data
        public IEnumerable<SelectListItem> TijdSlotOpties { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
