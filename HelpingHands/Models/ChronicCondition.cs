using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models
{
    public class ChronicCondition
    {
        [Key]
        public  int ChronicID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Archived { get; set; }

        public IList<PatientChronicCondition>? PatientCondition { get; set; }
    }

}
