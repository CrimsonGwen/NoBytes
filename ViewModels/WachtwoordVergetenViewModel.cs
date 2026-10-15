using System.ComponentModel.DataAnnotations;

namespace Restaurant.ViewModels
{
    public class WachtwoordVergetenViewModel
    {
        [Required(ErrorMessage = "E-mailadres is verplicht.")]
        [EmailAddress(ErrorMessage = "Voer een geldig e-mailadres in.")]
        public string Email { get; set; } = string.Empty;
    }
}
