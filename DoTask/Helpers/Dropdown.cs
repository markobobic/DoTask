using DoTask.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DoTask.Helpers
{
    public static class Dropdown
    {
        public static async Task<SortedSet<SelectListItem>> GenereteDropDownUsers(ApplicationDbContext db,string rolename)
        {
            SortedSet<SelectListItem> list = new SortedSet<SelectListItem>(new SelectListComparer());
            var data = await (from user in db.Users
                              from userRole in user.Roles
                              join role in db.Roles on userRole.RoleId equals role.Id
                              select new
                              {
                                  User = user,
                                  Role = role
                              }).ToListAsync();
            var dataBasedOnRoles = data.Where(x => x.Role.Name == rolename).ToList();
            try
            {
                for (int i = 0; i < dataBasedOnRoles.Count; i++)
                {
                    list.Add(dataBasedOnRoles.Select(x => new SelectListItem
                    {
                        Value = x.User.Id,
                        Text = x.User.FirstName + " " + x.User.LastName
                    }).FirstOrDefault(x => x.Value == dataBasedOnRoles[i].User.Id));
                }
                
                return list;
            }
            catch (Exception e)
            {

                throw e;
            }

        }


        public static async Task<List<SelectListItem>> GenerateDropdownProjects(ApplicationDbContext db,[Optional] Project project)
        {
            List<SelectListItem> data = new List<SelectListItem>();

            if (project == null)
            {
                data = await db.Projects.Select(proj => new SelectListItem()
                {
                    Value = proj.Id.ToString(),
                    Text = proj.Name
                }).ToListAsync();
                return data;
            }
            data = await db.Projects.Select(proj => new SelectListItem()
            {
                Value = proj.Id.ToString(),
                Text = proj.Name
            }).ToListAsync();
            SelectListItem currentProject = new SelectListItem() { Value = project.Id.ToString(), Text = project.Name, Selected = true };

            SelectListItem none = CreateNoneForDropdown();
            data.Remove(data.Single(x => x.Value == project.Id.ToString()));
            data.Insert(0, currentProject);
            if (project == null)
            {
                data.Insert(1, none);
            }
            return data;
        }

        public static async Task<List<SelectListItem>> GenerateDropdownProjectsForDEV(ApplicationDbContext db, [Optional] Project project)
        {
            List<SelectListItem> data = new List<SelectListItem>();

            if (project == null)
            {
                data = await db.Projects.Where(x=>x.ProjectManager!=null).Select(proj => new SelectListItem()
                {
                    Value = proj.Id.ToString(),
                    Text = proj.Name
                }).ToListAsync();
                if (data.Count == 0)
                {
                    SelectListItem noProjects = new SelectListItem() { Value="None", Disabled=true, Text = "There are no projects with assigned project manager", Selected = true };
                    data.Add(noProjects);
                    return data;
                }

                return data;
            }
            data = await db.Projects.Where(x => x.ProjectManager != null).Select(proj => new SelectListItem()
            {
                Value = proj.Id.ToString(),
                Text = proj.Name
            }).ToListAsync();
            if (data.Count == 0)
            {
                SelectListItem noProjects = new SelectListItem() { Value = "None", Disabled = true, Text = "There are no projects with assigned project manager", Selected = true };
                data.Add(noProjects);
                return data;
            }

            SelectListItem currentProject = new SelectListItem() { Value = project.Id.ToString(), Text = project.Name, Selected = true };
            SelectListItem none = CreateNoneForDropdown();
            data.Add(currentProject);
            
            return data;
        }





        public static SelectListItem CreateNoneForDropdown()
        {
            SelectListItem none = new SelectListItem() { Value = "None", Text = "None", Selected = true };
            return none;

        }

    }
}