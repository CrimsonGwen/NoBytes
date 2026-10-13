namespace Restaurant.ViewModels
{
    public class CreateAccountViewModel
    {
        [Required]
        public string Voornaam { get; set; } = string.Empty;
        [Required]
        public string Achternaam { get; set; } = string.Empty;
        [Required]
        public string Adres { get; set; } = string.Empty;
        [Required]
        public string Huisnummer { get; set; } = string.Empty;
        [Required]
        public string Postcode { get; set; } = string.Empty;
        [Required]
        public string Gemeente { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Wachtwoord { get; set; } = string.Empty;
        public int LandId { get; set; }
        public IEnumerable<SelectListItem>? Landen { get; set; }
    }
}
