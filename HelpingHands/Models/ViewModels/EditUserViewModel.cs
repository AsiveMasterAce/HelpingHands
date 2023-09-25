using System.ComponentModel.DataAnnotations;
namespace HelpingHands.Models.ViewModels
{
    public class EditUserViewModel
    {
        public int UserID { get; set; }
        [Display(Name = "User Type")]
        public string UserType { get; set; }
        [Required]
        [Display(Name = "First Name")]
        [StringLength(120, ErrorMessage = "The {0} must be at least {2} and at a max {1} characters long.", MinimumLength = 2)]
        public string? FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        [StringLength(120, ErrorMessage = "The {0} must be at least {2} and at a max {1} characters long.", MinimumLength = 2)]
        public string? LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$", ErrorMessage = "Invalid E-mail Address Format.")]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Cell Number")]
        [RegularExpression(@"^(\+27|0)[0-9]{9}$", ErrorMessage = "Invalid Cell Number Format.")]
        public string? CellNo { get; set; }
    }
}
