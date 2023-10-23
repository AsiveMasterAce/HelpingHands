using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models.ViewModels
{
    public class CareContractViewModel
    {
    }
    public class CreateContract
    {

        [Required]
        [Display(Name = "Wound Description")]
        public string? WoundDescription { get; set; }
        [Required]
        [Display(Name = "House Number")]
        public string? AddressLine1 { get; set; }
        [Required]
        [Display(Name = "Street Address")]
        public string? AddressLine2 { get; set; }

        [Display(Name = "City")]
        public int SelectedCityId { get; set; }

        [Required]
        [Display(Name = "Suburb")]
        public int SelectedSuburbId { get; set; }


    }


}
