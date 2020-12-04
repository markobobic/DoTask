using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoTask.VievModels
{
    public class UserUpdateViewModel
    {
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; }

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

        private string confirmPassword;
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = "Password must be between 5 and 100 characters!")]
        [Display(Name = "Confirm password")]
        [Compare("PasswordHash", ErrorMessage = "The password and confirmation password do not match.")]
        
        public string ConfirmPassword
        {
            get { return PasswordHash; }
            set { confirmPassword = value; }
        }

        public byte[] Photo { get; set; }
        public string PhotoType { get; set; }

        public UserUpdateViewModel(string id, string username, string email, string userRole, string firstName,
           string lastName,string passwordHash,string confirmPassowrd,byte[] photo,string photoType)
        {
            Id = id;
            UserName = username;
            Email = email;
            UserRole = userRole;
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;
            ConfirmPassword = confirmPassowrd;
            Photo = photo;
            PhotoType = photoType;
        }
        public UserUpdateViewModel(string id, string username, string email, string userRole, string firstName,
          string lastName, string passwordHash, string confirmPassowrd)
        {
            Id = id;
            UserName = username;
            Email = email;
            UserRole = userRole;
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;
            ConfirmPassword = confirmPassowrd;
            
        }
        public UserUpdateViewModel()
        {

        }
    }
}