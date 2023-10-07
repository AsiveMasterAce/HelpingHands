using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models.ViewModels
{
    public class UpdateSuburbVM
    {
        public int suburbID { get; set; }
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
