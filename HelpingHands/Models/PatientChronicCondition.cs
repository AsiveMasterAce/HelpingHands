using HelpingHands.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpingHands.Models
{
    public class PatientChronicCondition
    {
                
        
        [ForeignKey("ChronicCondition")]
        public int ChronicID { get; set; }
        
        [ForeignKey("Patient")]
        public int PatientID { get; set; }

        public virtual Patient? Patient { get; set; }
        public virtual ChronicCondition? ChronicCondition { get; set; }
    }
}
