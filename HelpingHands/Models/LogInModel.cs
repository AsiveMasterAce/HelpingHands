using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models
{
    public class LogInModel
    {

        [Required]
        [DataType(DataType.EmailAddress)]
        
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$", ErrorMessage = "Invalid E-mail Address.")]
        public string ?Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string ?Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
