﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpingHands.Models.Users
{
    public class Nurse
    {
        [Key]
        public int NurseID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? CellNo { get; set; }
        public string? Password { get; set; }
        public string? Gender { get; set; }
   
        public string? IDNumber { get; set; }

        public bool Archived { get; set; }
        [ForeignKey("User")]
        public int userID { get;set; }

        public string? ProfilePicUrl { get; set; }
        public virtual UserModel? User { get; set; }
        public IList<PreferredSuburb>? PreferredSuburbs { get; set; }
        public IList<CareContract>? CareContracts { get; set; }
    }
}
