using DoTask.Helpers;
using DoTask.Models;
using DoTask.Repository;
using DoTask.VievModels;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DoTask.Services
{
    public class AssignmentService : GenericRepo<Assignment>, IAssignmentService
    {
        private readonly ApplicationDbContext db;
        public AssignmentService(ApplicationDbContext _db) : base(_db)
        {
            db = _db;
        }

        public void CreateAssignment(Assignment assignment)
        {
            Create(assignment);
        }


        public void DeleteAssignment(Assignment assignment)
        {
            Delete(assignment);
        }

        public async Task<IEnumerable<Assignment>> GetAllIncludingAll()
        {
            return await FindAll()
                .OrderBy(assignment => assignment.Name).Include(x=>x.Project).Include(x=>x.Project.ProjectManager)
                .Include(x=>x.Status).Include(x=>x.Developers)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assignment>> GetAllTasksAsync()
        {
            return await FindAll()
                .OrderBy(assignment => assignment.Name)
                .ToListAsync();
        }

        public async Task<Assignment> GetAssignmentIdAsync(int assignmentId)
        {
            return await FindByCondition(assignment => assignment.Id.Equals(assignmentId)).Include(x=>x.Developers).Include(x=>x.Project.ProjectManager)
           .FirstOrDefaultAsync();
        }

        public async Task<Assignment> GetAssignmentWithProjectAndUserAsync(int assignmentId)
        {
            return await FindByCondition(assignment => assignment.Id.Equals(assignmentId))
               .Include(x=>x.Project)
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

                throw e.InnerException;
            }
            
        }

        public void UpdateAssignment(Assignment assignment)
        {
            Update(assignment);
        }
        public async Task<List<SelectListItem>> IncludeStatuses([Optional] int statusId)
        {
            var stat = await db.Statuses.SingleOrDefaultAsync(x => x.Id == statusId);
            List<SelectListItem> data = new List<SelectListItem>();
            if (stat ==null) { 
             data = await db.Statuses.Select(status => new SelectListItem
            {
                Value = status.Id.ToString(),
                Text = status.Name
            }).ToListAsync();
                return data;
            }
            data = await db.Statuses.Select(status => new SelectListItem
            {
                Value = status.Id.ToString(),
                Text = status.Name
            }).ToListAsync();
            SelectListItem currentStatus = new SelectListItem() { Value = stat.Id.ToString(), Text = stat.Name, Selected = true };
            data.Remove(data.Single(x => x.Value == stat.Id.ToString()));
            data.Insert(0, currentStatus);
            return data;
        }
      
        public async Task<List<SelectListItem>> IncludeProjects([Optional] int? projectId)
        {
            var currentProject =await db.Projects.SingleOrDefaultAsync(x => x.Id == projectId);
            var data = await GenerateDropdownProjects(currentProject);
            return data;
        }
        private async Task<List<SelectListItem>> GenerateDropdownProjects([Optional] Project project)
        {
            List<SelectListItem> data = new List<SelectListItem>();

            if (project == null) {
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
            data.Insert(0,currentProject);
            data.Insert(1, none);
            return data;
        }
        public async Task<Assignment> MapData(AssignmentViewModel viewModel)
        {
            if (viewModel != null)
            {
                if (viewModel.DeveloperId != null)
                {
                    var assignment = new Assignment();
                    var project = await db.Projects.Where(x => x.Id == viewModel.ProjectId).Include(x => x.Tasks).Include(x => x.ProjectManager).FirstOrDefaultAsync();
                    var status = await db.Statuses.SingleOrDefaultAsync(x => x.Id == viewModel.StatusId);
                    var developer = await db.Users.SingleOrDefaultAsync(x => x.Id == viewModel.DeveloperId);
                    assignment.Name = viewModel.AssigmentName;
                    assignment.Deadline = viewModel.Deadline;
                    assignment.Decription = viewModel.Description;
                    assignment.Progress = viewModel.Progress;
                    assignment.Project = project;
                    assignment.Status = status;
                    assignment.Developers.Add(developer);
                    return assignment;
                }
                else
                {
                    var assignment = new Assignment();
                    var project = await db.Projects.Where(x => x.Id == viewModel.ProjectId).Include(x => x.Tasks).Include(x => x.ProjectManager).FirstOrDefaultAsync();
                    var status = await db.Statuses.SingleOrDefaultAsync(x => x.Id == viewModel.StatusId);
                    var projectManager = await db.Users.SingleOrDefaultAsync(x => x.Id == viewModel.ProjectManagerId);
                    assignment.Name = viewModel.AssigmentName;
                    assignment.Deadline = viewModel.Deadline;
                    assignment.Decription = viewModel.Description;
                    assignment.Progress = viewModel.Progress;
                    assignment.Project = project;
                    assignment.Status = status;
                    project.Tasks.Add(assignment);
                    return assignment;
                }
            }
            return null;
        }

        public async Task<dynamic> GetAllProjectManagers()
        {
           var data = await (from user in db.Users
             from userRole in user.Roles
             join role in db.Roles on userRole.RoleId equals role.Id
             where role.Name == RoleName.ProjectManager
             select new
             {
                 Id = user.Id,
                 Name = user.FirstName + " " + user.LastName
             }).ToListAsync();
            return data;
        }

        public async Task<dynamic> GetProjectsWithProjectManagerId(string id)
        {
            var data =await db.Projects.Where(x => x.ProjectManagerId == id).Select(x => new { ProjectId = x.Id, Name = x.Name }).ToListAsync();
            return data;
        }

        public ApplicationUser GetDeveloperWithTask(Assignment assignment)
        {
            try
            {
                var developer = assignment.Developers.Count == 0 ? null : assignment.Developers.First(x => x.Assignments.Contains(assignment));
                return developer;
            }
            catch (Exception e)
            {

                throw e;
            }
           
        }
        public async Task<Tuple<SortedSet<SelectListItem>, SortedSet<SelectListItem>>> IncludeProjectManagersAndDevelopers()
        {
            var developers = await GenereteDropDownUsers(RoleName.Developer);
            var projectManagers = await GenereteDropDownUsers(RoleName.ProjectManager);
            Tuple<SortedSet<SelectListItem>, SortedSet<SelectListItem>> tuple = new Tuple<SortedSet<SelectListItem>, SortedSet<SelectListItem>>(developers, projectManagers);
            return tuple;
        }

        public async Task<SortedSet<SelectListItem>> IncludeOnlyDevelopers([Optional]ApplicationUser developer)
        {
            if(developer != null) { 
                SelectListItem currentDeveloper = new SelectListItem() { Value = developer.Id, Text = developer.FullName, Selected = true };
                SelectListItem none = CreateNoneForDropdown();
                var developersForUpdate = await GenereteDropDownUsers(RoleName.Developer);
                developersForUpdate.Remove(developersForUpdate.Single(x => x.Value == developer.Id));
                developersForUpdate.Add(currentDeveloper);
                developersForUpdate.Add(none);
                return developersForUpdate;
            }
            var developers = await GenereteDropDownUsers(RoleName.Developer);
            return developers;
        }

        public async Task<SortedSet<SelectListItem>> IncludeOnlyProjectManagers([Optional]ApplicationUser projectM)
        {
            SelectListItem currentProjectM = new SelectListItem() { Value = projectM.Id, Text = projectM.FullName, Selected = true };
            SelectListItem none = CreateNoneForDropdown();
            if (projectM != null) { 
                var projectManagersToUpdate = await GenereteDropDownUsers(RoleName.ProjectManager);
                projectManagersToUpdate.Remove(projectManagersToUpdate.Single(x => x.Value == projectM.Id));
                projectManagersToUpdate.Add(currentProjectM);
                projectManagersToUpdate.Add(none);
                return projectManagersToUpdate;

            }
            var projectManagers = await GenereteDropDownUsers(RoleName.ProjectManager);
            return projectManagers;
        }

        private async Task<SortedSet<SelectListItem>> GenereteDropDownUsers(string rolename)
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
        public async Task<ApplicationUser> GetProjectManagerWithTask(int? projectId)
        {
            var projectManager = await db.Projects.Where(x => x.Id == projectId).Select(x => x.ProjectManager).FirstOrDefaultAsync();
            return projectManager;
        }
        private SelectListItem CreateNoneForDropdown()
        {
            SelectListItem none = new SelectListItem() { Value = "0", Text = "None", Selected = false };
            return none;

        }
        public async Task<Assignment> UpdateMapDataForDeveloper(AssignmentUpdateViewModel viewModel)
        {
            var assignment = await GetAssignmentIdAsync(viewModel.Id);
            var project = await db.Projects.SingleOrDefaultAsync(x => x.Id == viewModel.ProjectId);
            var status = await db.Statuses.SingleOrDefaultAsync(x => x.Id == viewModel.StatusId);
            assignment.Deadline = viewModel.Deadline;
            assignment.Decription = viewModel.Description;
            var developer = GetDeveloperWithTask(assignment);
          
            assignment.Name = viewModel.AssigmentName;
            assignment.Progress = viewModel.Progress;
            assignment.Project = project;
            assignment.Status = status;
            if (developer != null)
            {
                try
                {
                    db.Assignments.Attach(assignment);
                    var newDeveloper = db.Users.Where(x => x.Id == viewModel.DeveloperId).First();
                    db.Entry(newDeveloper).Collection(x => x.Assignments).Load();
                    db.Entry(assignment).Collection(x => x.Developers).Load();
                    assignment.Developers.Add(newDeveloper);
                    assignment.Developers.Remove(developer);
                    developer.Assignments.Remove(assignment);
                    newDeveloper.Assignments.Add(assignment);
                }
                catch (Exception e)
                {

                    throw e;
                }
               
            }
            return assignment;


        }
       
        public void Nesto(int projectId)
        {
            throw new NotImplementedException();
        }
    }
}