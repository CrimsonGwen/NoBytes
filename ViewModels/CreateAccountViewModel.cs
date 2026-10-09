namespace Restaurant.ViewModels
{
    public class CreateAccountViewModel
    {
        [Required]
        public string Voornaam { get; set; }
        [Required]
        public string Achternaam { get; set; }
        [Required]
        public string Adres { get; set; }
        [Required]
        public string Huisnummer { get; set; }
        [Required]
        public string Postcode { get; set; }
        [Required]
        public string Gemeente { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Wachtwoord { get; set; }
    }
}
