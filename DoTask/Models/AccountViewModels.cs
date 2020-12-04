using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoTask.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }
   
    
    public class LoginViewModel
    {
        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 4, ErrorMessage = "User name must be between 4 and 50 characters!")]
        [RegularExpression(@"^(\d|\w)+$", ErrorMessage = "No spaces and special charachters!")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters!")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "User Roles")]
        public string UserRole { get; set; }

        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 4, ErrorMessage = "User name must be between 4 and 50 characters!")]
        [RegularExpression(@"^(\d|\w)+$", ErrorMessage = "No spaces and special charachters!")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [RegularExpression(@"^\w+( +\w+)*$|", ErrorMessage = "Spaces can only exist beetween words")]
        [Display(Name = "First Name")]
        [StringLength(maximumLength: 20, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 60 characters!")]
        [Required(ErrorMessage = "You must provide first name")]
        public string FirstName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [RegularExpression(@"^\w+( +\w+)*$|", ErrorMessage = "Spaces can only exist beetween words")]
        [Display(Name = "Last Name")]
        [StringLength(maximumLength: 50, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 20 characters!")]
        [Required(ErrorMessage = "You must provide last name")]
        public string LastName { get; set; }
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 20, MinimumLength = 6, ErrorMessage = "Password must be between 5 and 20 characters!")]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

   
}
