using System.ComponentModel.DataAnnotations;
namespace Restaurant.ViewModels

{
    public class LogInVieuwModel
    {
        [Required(ErrorMessage ="Gebruikernaam is verplicht.")]
        public string Gebruikersnaam { get; set; }
        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
