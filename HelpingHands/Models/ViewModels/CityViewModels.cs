using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models.ViewModels
{
    public class CityViewModels
    {
    }

    public class AddCityVM
    {
        [Required]
        [Display(Name = "City Name")]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "Short Name")]
        public string? Short { get; set; }

    }
    public class UpdateCityVM
    {
        public int CityId { get; set; }    
        [Required]
        [Display(Name = "City Name")]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "Short Name")]
        public string? Short { get; set; }
    }

}
