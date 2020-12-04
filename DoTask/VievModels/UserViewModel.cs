using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoTask.VievModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        [Display(Name = "User role")]
       
        public string UserRole { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [RegularExpression(@"^\w+( +\w+)*$|", ErrorMessage = "Spaces can only exist beetween words")]
        [Display(Name = "First Name")]
        [StringLength(maximumLength: 20, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 20 characters!")]
        [Required(ErrorMessage = "You must provide first name")]
        public string FirstName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [RegularExpression(@"^\w+( +\w+)*$|", ErrorMessage = "Spaces can only exist beetween words")]
        [Display(Name = "Last Name")]
        [StringLength(maximumLength: 50, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 20 characters!")]
        [Required(ErrorMessage = "You must provide last name")]
        public string LastName { get; set; }
        public byte[] Photo { get; set; }
        public string PhotoPath { get; set; }

       
    }
}