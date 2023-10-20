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
    public class UpdateUserPasswordViewModel
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

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
         ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again!")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }

    }

}
