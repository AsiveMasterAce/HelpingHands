using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpingHands.Models.ViewModels
{
    public class AddSuburbViewModel
    {
        [Required]
        [Display(Name = "Suburb Name")]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "Postal Code")]
        public int PostalCode { get; set; }
        [Display(Name = "City")]
        public int CityID { get; set; }
    }
}
