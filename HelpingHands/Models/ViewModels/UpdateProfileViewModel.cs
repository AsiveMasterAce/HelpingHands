using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models.ViewModels
{
    public class UpdateProfileViewModel
    {
    }
    public class UpdateNurseProfileViewModel
    {
        public int NurseId { get; set; }

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
        [Required]
        [RegularExpression(@"(([0-9]{2})(0|1)([0-9])([0-3])([0-9]))([ ]?)(([0-9]{4})([ ]?)([0-1][8]([ ]?)[0-9]))", ErrorMessage = "Invalid ID number.")]
        [Display(Name = "ID Number")]
        public string? IDNumber { get; set; }

        [Required]
        [Display(Name = "Gender")]

        public string? Gender { get; set; }

    }

    public class UpdatePatientsProfileViewModel
    {
        public int PatientID { get; set; }
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
        [Required]
        [RegularExpression(@"(([0-9]{2})(0|1)([0-9])([0-3])([0-9]))([ ]?)(([0-9]{4})([ ]?)([0-1][8]([ ]?)[0-9]))", ErrorMessage = "Invalid ID number.")]
        [Display(Name = "ID Number")]
        public string? IDNumber { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }
        [Required]
        [Display(Name = "Emergency Person Name")]
        public string? EmergencyPerson { get; set; }
        [Required]
        [Display(Name = "Emergency Person Number")]
        [RegularExpression(@"^(\+27|0)[0-9]{9}$", ErrorMessage = "Invalid Cell Number Format.")]
        public string? EmergencyPersonNo { get; set; }


    }

    public class UpdatePasswordViewModel
    {
        [Required(ErrorMessage = "Old Password is required.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New Password is required.")]
        [StringLength(100, ErrorMessage = "New Password must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm New Password is required.")]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirm New Password must match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
