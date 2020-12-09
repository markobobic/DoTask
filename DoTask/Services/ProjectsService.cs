using DoTask.Helpers;
using DoTask.Models;
using DoTask.Repository;
using DoTask.VievModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DoTask.Services
{
    public class ProjectsService : GenericRepo<Project>, IProjectService
    {
        private readonly ApplicationDbContext db;
        public ProjectsService(ApplicationDbContext _db) :base(_db)
        {
            db = _db;
        }
        public void CreateProject(Project project)
        {
            Create(project);
        }

        public void DeleteProject(Project project)
        {
            Delete(project);
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await FindAll()
               .OrderBy(project => project.Name)
               .ToListAsync();
        }
        public async Task<IEnumerable<Project>> GetAllProjectssWithProjectManager()
        {
            return await FindAll()
              .OrderBy(project => project.Name).Include(x=>x.ProjectManager)
              .ToListAsync();
        }
        public async Task<IEnumerable<Project>> GetAllProjectssWithTasks()
        {
            return await FindAll()
              .OrderBy(project => project.Name).Include(x=>x.Tasks)
              .ToListAsync();
        }

        public async Task<Project> GetProjectByIdAsync(int projectId)
        {
            return await FindByCondition(project => project.Id.Equals(projectId))
           .FirstOrDefaultAsync();
        }
        public async Task<Project> GetProjectAndProjectManagertByIdAsync(int projectId)
        {
            return await FindByCondition(project => project.Id.Equals(projectId)).Include(x=>x.ProjectManager)
           .FirstOrDefaultAsync();
        }
        public async Task<Project> GetUserWithDetailsAsync(int projectId)
        {
            return await FindByCondition(project => project.Id.Equals(projectId)).Include(task=>task.Tasks)
                .Include(x=>x.ProjectManager)
            .FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw e;
            }
           
        }

        public void UpdateProject(Project project)
        {
            Update(project);
        }

        public async Task<SortedSet<SelectListItem>> IncludeProjectManagersDropdown([Optional] ApplicationUser projectM)
        {
            if (projectM != null)
            {
                SelectListItem currentProjectM = new SelectListItem() { Value = projectM.Id, Text = projectM.FullName, Selected = true };
                if (projectM != null)
                {
                    var projectManagersToUpdate = await Dropdown.GenereteDropDownUsers(db,RoleName.ProjectManager);
                    projectManagersToUpdate.Remove(projectManagersToUpdate.Single(x => x.Value == projectM.Id));
                    projectManagersToUpdate.Add(currentProjectM);
                    SelectListItem noneForUpdate = new SelectListItem() { Value = "None", Text = "None", Selected = false };
                    projectManagersToUpdate.Add(noneForUpdate);
                    return projectManagersToUpdate;

                }
                var projectManagers = await Dropdown.GenereteDropDownUsers(db, RoleName.ProjectManager);
                return projectManagers;
            }
            var projectManagersIfNone =  await Dropdown.GenereteDropDownUsers(db, RoleName.ProjectManager);
            SelectListItem none = Dropdown.CreateNoneForDropdown();
            projectManagersIfNone.Add(none);
            return projectManagersIfNone;
        }

        

        public async Task<Project> MapData(ProjectViewModel viewModel)
        {
            if (viewModel != null) { 
            var project = new Project();
            var projectManager = await db.Users.Where(x => x.Id == viewModel.ProjectManagerId).FirstOrDefaultAsync();
            project.Code = viewModel.Code;
            project.Name = viewModel.Name;
            project.ProjectManager = projectManager;
            return project;
            }
            return null;
        }
        public async Task<Project> UpdateMapData(ProjectUpdateViewModel updateViewModel,bool isNone)
        {
            var project =await db.Projects.Where(x => x.Id == updateViewModel.Id)
                .Include(x => x.ProjectManager).SingleOrDefaultAsync();
            var projectManager = await db.Users.Where(x => x.Id == updateViewModel.ProjectManagerId).FirstOrDefaultAsync();
            if(isNone ==true && projectManager != null)
            {
                var assignments = await db.Assignments.Where(x => x.ProjectId == project.Id && x.AssingToNone == true && x.Project.ProjectManager==null).ToListAsync();
                foreach(var assignment in assignments)
                {
                    assignment.AssingToNone = false;
                    db.Entry(assignment).Property(x => x.AssingToNone).IsModified = true;
                }
               await db.SaveChangesAsync();
            } else if(isNone == false && projectManager == null)
            {
                var assigmentsDevs = await db.Users.SelectMany(x => x.Assignments).Where(x => x.ProjectId == project.Id).ToListAsync();

                var noneAssigmentsForDev = await db.Assignments.Where(x => x.ProjectId == project.Id && x.ProjectId != null && x.AssingToNone == true).ToListAsync();
                if(assigmentsDevs.Count>0 && noneAssigmentsForDev.Count > 0)
                {
                    db.Assignments.RemoveRange(assigmentsDevs);
                    db.Assignments.RemoveRange(noneAssigmentsForDev);
                    await db.SaveChangesAsync();
                } else if(assigmentsDevs.Count>0 && noneAssigmentsForDev.Count == 0)
                {
                    db.Assignments.RemoveRange(assigmentsDevs);
                    await db.SaveChangesAsync();
                } else if(noneAssigmentsForDev.Count>0 && assigmentsDevs.Count == 0)
                {
                    db.Assignments.RemoveRange(noneAssigmentsForDev);
                    await db.SaveChangesAsync();
                }
            }
            project.Name = updateViewModel.Name;
            project.ProjectManager = projectManager;
            project.Code = updateViewModel.Code;
            return project;
        }

        public async Task<ApplicationUser> GetCurrentUser()
        {
          return await  System.Web.HttpContext.Current.GetOwinContext().
             GetUserManager<ApplicationUserManager>().FindByIdAsync(System.Web.HttpContext.Current.User.Identity.GetUserId());
        }


        public async Task<IEnumerable<ProjectIndexViewModel>> GetProjectDataAdminAsync()
        {
            var getAll = await GetAllProjectssWithProjectManager();
            var data =  getAll.Select(x => new ProjectIndexViewModel
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ProjectManager = x.ProjectManagerId == null ? "Unassigned" : $"{x.ProjectManager.FirstName} {x.ProjectManager.LastName}"
            });
            return data;
        }

        public async Task<IEnumerable<ProjectIndexProjectManagerViewModel>> GetProjectDataProjectManagerAsync()
        {
            var getAll = await GetAllProjectssWithProjectManager();
            var data = getAll.Select(x => new ProjectIndexProjectManagerViewModel
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ProjectManager = x.ProjectManagerId == null ?
                "Unassigned" : $"{x.ProjectManager.FirstName} {x.ProjectManager.LastName}",
                AverageProgress = x.Tasks.Sum(y=>y.Progress) /2
                
            });
            return data;
        }
    }
}
