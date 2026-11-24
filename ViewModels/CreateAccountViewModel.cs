namespace Restaurant.ViewModels
{
    public class CreateAccountViewModel
    {
        [Required(ErrorMessage = "Voer een voornaam in")]
        public string Voornaam { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een achternaam in")]
        public string Achternaam { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een gebruikersnaam in")]
        public string Gebruikersnaam { get; set; }
        [Required(ErrorMessage = "Voer een adres in")]
        public string Adres { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een huisnummer in")]
        public string Huisnummer { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een postcode in")]
        public string Postcode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een gemeente in")]
        public string Gemeente { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een email adres in")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Voer een geldig email adres in")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Voer een wachtwoord in")]
        [DataType(DataType.Password)]
        public string Wachtwoord { get; set; } = string.Empty;
        [Required(ErrorMessage = "Kies een land")]
        public int LandId { get; set; }
        public IEnumerable<SelectListItem>? Landen { get; set; }
    }
}
