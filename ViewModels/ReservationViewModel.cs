namespace Restaurant.ViewModels
{
    public class ReservationViewModel
    {
        [Required(ErrorMessage = "Selecteer een datum.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Selecteer een tijdslot.")]
        public int TijdSlotId { get; set; }

        [Required(ErrorMessage = "Geef het aantal personen op.")]
        [Range(1, 12, ErrorMessage = "Aantal personen moet tussen 1 en 12 zijn.")]
        public int Persons { get; set; } = 2;

        public string? SpecialRequests { get; set; }

        public IEnumerable<SelectListItem>? BeschikbareTijdSloten { get; set; }
        public List<string> AlternativeSlots { get; set; } = new();
    }
}
