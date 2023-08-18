using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public bool Archived { get; set; }

    }
}
