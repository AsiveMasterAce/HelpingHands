using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models.ViewModels
{
    public class CareVisitViewModel
    {


    }

    public class AddVisitViewModel
    {
        [Required]
        [Display(Name = "Visit Date")]
        public DateTime? VisitDate { get; set; }

        [Required]
        [Display(Name = "Axtimate Arrive Time")]
        public TimeSpan? AxtimateArriveTime { get; set; }

        public int contractId { get; set; }
    }

    public class AddArriveTimeViewModel
    {
        [Required]
        [Display(Name = "Visit Arrive Time")]
        public TimeSpan? VisitArriveTime { get; set; }

        public int CareVisitId { get; set; }
    }
    public class AddNotesTimeViewModel
    {
        [Required]
        [Display(Name = "Wound Condition")]
        public string? WoundCondition { get; set; }
        [Required]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public int CareVisitId { get; set; }
    }


    public class AddDepartTimeViewModel
    {
        [Required]
        [Display(Name = "Visit Depart Time")]
        public TimeSpan? VisitDepartTime { get; set; }

        public int CareVisitId { get; set; }
    }
    public class VisitViewModel
    {
        public int VisitId { get; set; }
        public DateTime? VisitDate { get; set; }
        public string? WoundCondition { get; set; }
        public string? Notes { get; set; }

    }

}
