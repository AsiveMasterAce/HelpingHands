using HelpingHands.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpingHands.Models
{
    public class PreferredSuburb
    {

        [ForeignKey("Suburb")]
        public int SuburbID { get; set; }
        [ForeignKey("Nurse")]
        public int NurseID { get; set; }
        public bool? Archived { get; set; }
        public virtual Nurse? Nurse { get; set; }
        public virtual Suburb? Suburb { get; set; }



    }
}
