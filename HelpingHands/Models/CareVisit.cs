using Microsoft.EntityFrameworkCore.Query.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpingHands.Models
{
    public class CareVisit
    {
        [Key]
        public int CareVisitID { get; set; }
        public DateTime VisitDate { get;set; }
        public DateTime AxtimateArriveTime { get; set; }
        public DateTime VisitArriveTime { get; set; }
        public DateTime VisitDepartTime { get; set; }
        public string? WoundCondition { get; set; }
        public string? Notes { get; set; }
        [ForeignKey("CareContract")]
        public int ContractID  { get; set; }

        public bool Archived { get; set; }

        public virtual CareContract? CareContract { get; set; }
    }
}
