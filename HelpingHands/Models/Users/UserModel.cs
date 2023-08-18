using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models.Users
{
    public class UserModel
    {
        [Key]
        public int UserID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? CellNo { get; set; }
        public string? UserType { get; set; }
        public bool Archived { get; set; }
    }
}
