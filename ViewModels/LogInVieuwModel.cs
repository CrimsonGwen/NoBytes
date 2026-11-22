using System.ComponentModel.DataAnnotations;
namespace Restaurant.ViewModels

{
    public class LogInVieuwModel
    {
        [Required(ErrorMessage ="Gebruikernaam is verplicht.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
