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

            var admin = new ApplicationUser
            {
                Email = "admin@myemail.com",
                UserName = "admin",
                FirstName = "Jovan",
                LastName = "Jovic",
                LastActivity = DateTime.Now,
                IsActive = true
            };
            var developer = new ApplicationUser
            {
                Email = "developer@gmail.com",
                UserName = "developer",
                FirstName = "Mirko",
                LastName = "Mirkovic",
                LastActivity = DateTime.Now,
                IsActive = true
            };
            var projectManager = new ApplicationUser
            {
                Email = "projectm@gmail.com",
                UserName = "projectm",
                FirstName = "Ivana",
                LastName = "Ivanovic",
                LastActivity = DateTime.Now,
                IsActive = true
            };

            manager.Create(admin, "admin123");
            manager.Create(developer, "developer123");
            manager.Create(projectManager, "projectm123");
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            roleManager.Create(new IdentityRole("Admin"));
            roleManager.Create(new IdentityRole("ProjectManager"));
            roleManager.Create(new IdentityRole("Developer"));
            manager.AddToRole(admin.Id, "Admin");
            manager.AddToRole(developer.Id, "Developer");
            manager.AddToRole(projectManager.Id, "ProjectManager");

        }
    }
}