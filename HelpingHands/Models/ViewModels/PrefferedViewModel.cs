using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models.ViewModels
{
    public class PrefferedViewModel
    {
    }
    public class AddPrefferedViewModel 
    {
        [Display(Name = "City")]
        public int SelectedCityId { get; set; }

        [Required]
        [Display(Name = "Suburb")]
        public int SelectedSuburbId { get; set; }
    }

}
