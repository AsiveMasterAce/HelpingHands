using HelpingHands.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;

namespace HelpingHands.Models
{
    public class CareContract
    {
        [Key]
        public int ContractID { get; set; }
        public DateTime ContractDate { get; set; }
        public string? WoundDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? CareStatus { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public bool Archived { get; set; }

        [ForeignKey("Suburb")]
        public int SuburbID { get; set; }
        [ForeignKey("Nurse")]
        public int NurseID { get; set; }
        [ForeignKey("Patient")]
        public int PatientID { get; set; }

        public virtual Nurse? Nurse { get; set; }
        public virtual Suburb? Suburb { get; set; }
        public virtual Patient? Patient { get; set; }


    }
}
