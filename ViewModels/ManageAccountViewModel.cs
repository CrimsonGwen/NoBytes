using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Restaurant.ViewModels
{
    public class ManageAccountViewModel
    {
        [Required]
        [Display(Name = "Voornaam")]
        public string Voornaam { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Achternaam")]
        public string Achternaam { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Gebruikersnaam")]
        public string Gebruikersnaam { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "E-mailadres")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Adres { get; set; } = string.Empty;

        [Required]
        public string Huisnummer { get; set; } = string.Empty;

        [Required]
        public string Postcode { get; set; } = string.Empty;

        [Required]
        public string Gemeente { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Selecteer een land.")]
        [Display(Name = "Land")]
        public int LandId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Landen { get; set; }
            = new List<SelectListItem>();
    }
}