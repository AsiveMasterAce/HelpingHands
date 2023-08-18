﻿using System.ComponentModel.DataAnnotations.Schema;

namespace HelpingHands.Models
{
    public class Suburb
    {
        public int SuburbID { get; set; }
        public string? Name { get; set; }
        public int PostalCode { get; set; }

        [ForeignKey("City")]
        public int CityID { get; set; }
        public bool Archived { get; set; }
        public virtual City? City { get; set; }

        public IList<PreferredSuburb>? PreferredSuburbs { get; set; }
    }
}