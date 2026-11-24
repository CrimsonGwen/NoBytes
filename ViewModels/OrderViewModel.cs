using System.ComponentModel.DataAnnotations;
namespace Restaurant.ViewModels

{
    public class OrderViewModel
    {
        public string Naam { get; set; } = string.Empty;
        public string Beschrijving { get; set; } = string.Empty;
        public string AllergenenInfo { get; set; } = string.Empty;
        public decimal Prijs { get; set; }

        
    }
}
