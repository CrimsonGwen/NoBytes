namespace Restaurant.ViewModels
{
    public class CreateAccountViewModel
    {
        [Required(ErrorMessage = "Voer een voornaam in")]
        public string Voornaam { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een Achternaam in")]
        public string Achternaam { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een Adres in")]
        public string Adres { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een Huisnummer in")]
        public string Huisnummer { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een Postcode in")]
        public string Postcode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een Gemeente in")]
        public string Gemeente { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een email adres in")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Voer een geldig email adres in")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een wachtwoord in")]
        [DataType(DataType.Password)]
        public string Wachtwoord { get; set; } = string.Empty;
        [Required(ErrorMessage = "Bevestig je wachtwoord")]
        [DataType(DataType.Password)]
        [Compare(nameof(Wachtwoord), ErrorMessage = "De wachtwoorden komen niet overeen")]
        public string BevestigWachtwoord { get; set; } = string.Empty;
        public int LandId { get; set; }
        public IEnumerable<SelectListItem>? Landen { get; set; }
    }
}
