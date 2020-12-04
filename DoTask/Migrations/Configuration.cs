namespace DoTask.Migrations
{
    using DoTask.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DoTask.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DoTask.Models.ApplicationDbContext context)
        {
            ApplicationUserManager manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var user = new ApplicationUser { Email = "admin@myemail.com", UserName = "admin", FirstName = "Nikola",
                LastName = "Nikolic", IsActive = true,LastActivity=DateTime.Now
            };
            var projectManager = new ApplicationUser
            {
                Email = "projectm@myemail.com",
                UserName = "projectm",
                FirstName = "Mirko",
                LastName = "Mirkovic",
                IsActive = true,
                LastActivity = DateTime.Now
            };

            var developer = new ApplicationUser
            {
                Email = "dev@myemail.com",
                UserName = "developer",
                FirstName = "Igor",
                LastName = "Igorovic",
                IsActive = true,
                LastActivity = DateTime.Now
            };

            manager.Create(user, "admin123");
            manager.Create(projectManager, "projectm123");
            manager.Create(developer, "developer123");

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            roleManager.Create(new IdentityRole("Admin"));
            roleManager.Create(new IdentityRole("ProjectManager"));
            roleManager.Create(new IdentityRole("Developer"));

            manager.AddToRole(user.Id, "Admin");

            manager.AddToRole(projectManager.Id, "ProjectManager");

            manager.AddToRole(developer.Id, "Developer");

        }
    }
}
