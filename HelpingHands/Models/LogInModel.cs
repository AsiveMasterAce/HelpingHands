using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models
{
    public class LogInModel
    {

        [Required]
        [EmailAddress]
        public string ?Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string ?Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
