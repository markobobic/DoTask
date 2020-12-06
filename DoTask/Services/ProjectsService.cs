using DoTask.Models;
using DoTask.Repository;
using DoTask.VievModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<SelectList> IncludeRoles()
        {
            var data = await (from user in db.Users
                              from userRole in user.Roles
                              join role in db.Roles on userRole.RoleId equals role.Id
                              where role.Name == "ProjectManager"
                              select new
                              {
                                  ProjectManagerId = user.Id,
                                  FullName = user.FirstName + " " + user.LastName
                              }).ToListAsync();


            return new SelectList(data, "ProjectManagerId", "FullName");
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
        public async Task<Project> UpdateMapData(ProjectUpdateViewModel updateViewModel)
        {
            var project =await db.Projects.Where(x => x.Id == updateViewModel.Id)
                .Include(x => x.ProjectManager).SingleOrDefaultAsync();
            var projectManager = await db.Users.Where(x => x.Id == updateViewModel.ProjectManagerId).FirstOrDefaultAsync();

            project.Name = updateViewModel.Name;
            project.ProjectManager = projectManager;
            project.Code = updateViewModel.Code;
            return project;
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
