using System.ComponentModel.DataAnnotations;

namespace Restaurant.ViewModels
{
    public class WachtwoordResettenViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nieuw wachtwoord is verplicht.")]
        [DataType(DataType.Password)]
        public string NieuwWachtwoord { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bevestig je nieuwe wachtwoord.")]
        [DataType(DataType.Password)]
        [Compare(
            "NieuwWachtwoord",
            ErrorMessage = "De wachtwoorden komen niet overeen."
        )]
        public string BevestigWachtwoord { get; set; } = string.Empty;
    }
}
