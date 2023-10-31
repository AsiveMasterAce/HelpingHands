using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpingHands.Models.Users
{
    public class Patient
    {

        [Key]
        public int PatientID { get; set; }
        public string? FirstName { get; set; }
        public string?LastName { get; set; }
        public string? Email { get; set; }
        public string? CellNo { get; set; }
        public string? Password { get; set; }
        public DateTime DOB { get; set; }
        public string? IDNumber { get; set; }
        public string? EmergencyPerson { get; set; }
        public string? EmergencyPersonNo { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set;}
        public string? ProfilePicUrl { get; set; }

        [ForeignKey("Suburb")]
        public int SuburbID { get; set; }

        [ForeignKey("User")]
        public int userID { get; set; }

        public bool Archived { get; set; }


        public virtual UserModel? User { get; set; }

        public virtual Suburb? Suburb { get; set; }

        public IList<PatientChronicCondition>? PatientCondition { get; set; }
        public IList<CareContract>? CareContracts { get; set; }
    }
}
