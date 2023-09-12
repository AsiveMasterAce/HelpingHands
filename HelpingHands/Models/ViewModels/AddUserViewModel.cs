using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models.ViewModels
{
    public class AddUserViewModel
    {
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
        [DataType(DataType.Password)]
        public string? Password { get; set; }
      
        [Required]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }
        [Required]
        [RegularExpression(@"^(?:(?:\(|)0|\+27|27)(?:1[12345678]|2[123478]|3[1234569]|4[\d]|5[134678])(?:\) | |-|)\d{3}(?: |-|)\d{4}$", ErrorMessage = "Invalid Cell Number Format.")]
        public string? CellNo { get; set; }
        [Required]
        public string? UserType { get; set; }
    }
}
