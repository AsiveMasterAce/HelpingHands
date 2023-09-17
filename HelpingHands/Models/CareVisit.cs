using Microsoft.EntityFrameworkCore.Query.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpingHands.Models
{
    public class CareVisit
    {
        [Key]
        public int CareVisitID { get; set; }
        public DateTime? VisitDate { get;set; }
        public TimeSpan? AxtimateArriveTime { get; set; }
        public TimeSpan? VisitArriveTime { get; set; }
        public TimeSpan? VisitDepartTime { get; set; }
        public string? WoundCondition { get; set; }
        public string? Notes { get; set; }
        [ForeignKey("CareContract")]
        public int? ContractID  { get; set; }

        public bool? Archived { get; set; }

        public virtual CareContract? CareContract { get; set; }
    }
}
