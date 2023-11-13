using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models.ViewModels
{
    public class ChronicConditionViewModel
    {

    }
    public class AddChronicVM 
    {
        [Required]
        [Display(Name = "Name")]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }

    public class EditChronicVM
    {
        public int ChronicID { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
    public class SelectChronicVM
    {
        public IEnumerable<int> SelectedChronicConditions { get; set; }
    }
    public class PatientChronicConditionViewModel
    {
        public int PatientID { get; set; }
        public string FullName { get; set; }

        public List<string> ChronicConditions { get; set; }
    }

}
