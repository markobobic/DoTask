using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DoTask.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public DateTimeOffset LastActivity { get; set; }
        private string fulLName;
        [NotMapped]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
            set { fulLName = value; }
        }

        [NotMapped]
        [Display(Name = "User role")]
        public string UserRole { get; set; }
        public bool IsActive { get; set; }
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
        public string PhotoType { get; set; }
        public byte[] Photo { get; set; }

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

        

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            this.Configuration.AutoDetectChangesEnabled = true;

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}

