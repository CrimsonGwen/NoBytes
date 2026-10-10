namespace Restaurant.ViewModels
{
    public class LogInVieuwModel
    {
        [Required(ErrorMessage ="Email is verplicht.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
