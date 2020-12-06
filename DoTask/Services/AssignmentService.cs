using DoTask.Helpers;
using DoTask.Models;
using DoTask.Repository;
using DoTask.VievModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
            return await FindByCondition(assignment => assignment.Id.Equals(assignmentId)).Include(x=>x.Developers).Include(x=>x.Project)
           .FirstOrDefaultAsync();
        }
        public async Task<Assignment> GetAssignmentWithDevelopers(int assignmentId)
        {
            return await FindByCondition(assignment => assignment.Id.Equals(assignmentId)).Include(x => x.Developers)
           .FirstOrDefaultAsync();
        }

        public async Task<Assignment> GetAssignmentWithProjectAndProjectManagerAsync(int assignmentId)
        {
            try
            {
                return await FindByCondition(assignment => assignment.Id.Equals(assignmentId))
               .Include(x => x.Project).Include(x => x.Project.ProjectManager)
           .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {

                throw;
            }
            
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
        public async Task<List<SelectListItem>> IncludeStatusesDropdown([Optional] int statusId)
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
      
        public async Task<List<SelectListItem>> IncludeProjectsDropdown([Optional] int? projectId)
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
            if (project == null) { 
             data.Insert(1, none);
            }
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
                    assignment.Name = viewModel.AssigmentName;
                    assignment.Deadline = viewModel.Deadline;
                    assignment.Decription = viewModel.Description;
                    assignment.Progress = viewModel.Progress;
                    assignment.Project = project;
                    assignment.Status = status;
                    assignment.StartDate = viewModel.StartDate;
                    if(viewModel.DeveloperId != "None")
                    {
                        var developer = await db.Users.SingleOrDefaultAsync(x => x.Id == viewModel.DeveloperId);
                        assignment.Developers.Add(developer);
                        assignment.AssingToNone = false;
                        return assignment;
                    }
                    assignment.AssingToNone = true;
                    return assignment;
                }
                else if (viewModel.ProjectManagerId != null)
                {
                    var assignment = new Assignment();
                    var status = await db.Statuses.SingleOrDefaultAsync(x => x.Id == viewModel.StatusId);
                    assignment.Name = viewModel.AssigmentName;
                    assignment.Deadline = viewModel.Deadline;
                    assignment.Decription = viewModel.Description;
                    assignment.Progress = viewModel.Progress;
                    if (viewModel.ProjectManagerId != "None")
                    {
                      var project = await db.Projects.Where(x => x.Id == viewModel.ProjectId).Include(x => x.Tasks).Include(x => x.ProjectManager).FirstOrDefaultAsync();
                      assignment.Project = project;
                      assignment.Status = status;
                      project.Tasks.Add(assignment);
                      assignment.AssingToNone = false;
                      return assignment;
                    }
                    assignment.Status = status;
                    assignment.AssingToNone = true;
                    assignment.Project = null;
                    assignment.ProjectId = null;
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

        public async Task<List<CascadeDropdownProjectViewModel>> GetProjectsWithProjectManagerId(string id)
        {
            var data =await db.Projects.Where(x => x.ProjectManagerId == id).Select(x => new CascadeDropdownProjectViewModel { ProjectId = x.Id, Name = x.Name }).ToListAsync();
            return data;
        }

        public ApplicationUser GetDeveloperWithTask(Assignment assignment)
        {
            try
            {
                var developer = assignment.Developers.Count == 0 ? null : assignment.Developers.FirstOrDefault(x => x.Assignments.Contains(assignment));
                return developer;
            }
            catch (Exception e)
            {

                throw e;
            }
           
        }
        public async Task<Tuple<SortedSet<SelectListItem>, SortedSet<SelectListItem>>> IncludeProjectManagersAndDevelopersDropdown()
        {
            var developers = await GenereteDropDownUsers(RoleName.Developer);
            var none = CreateNoneForDropdown();
            developers.Add(none);
            var projectManagers = await GenereteDropDownUsers(RoleName.ProjectManager);
            Tuple<SortedSet<SelectListItem>, SortedSet<SelectListItem>> tuple = new Tuple<SortedSet<SelectListItem>, SortedSet<SelectListItem>>(developers, projectManagers);
            return tuple;
        }

        public async Task<SortedSet<SelectListItem>> IncludeDevelopersDropdown([Optional] bool isNone,[Optional]ApplicationUser developer)
        {
            SelectListItem currentDeveloper = isNone == false ? new SelectListItem() { Value = developer.Id, Text = developer.FullName, Selected = true }
                                                : new SelectListItem() { Value = "None", Text = "None", Selected = true };
            if (developer != null) {
                var developersForUpdate = await GenereteDropDownUsers(RoleName.Developer);
                developersForUpdate.Remove(developersForUpdate.Single(x => x.Value == developer.Id));
                developersForUpdate.Add(currentDeveloper);
              
                return developersForUpdate;
            }
            var developers = await GenereteDropDownUsers(RoleName.Developer);
            developers.Add(currentDeveloper);
          
            return developers;
        }

        public async Task<SortedSet<SelectListItem>> IncludeProjectManagersDropdown([Optional]ApplicationUser projectM)
        {
            if(projectM != null) { 
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
            var projectManagersIfNone = await GenereteDropDownUsers(RoleName.ProjectManager);
            return projectManagersIfNone;
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
        public async Task Unassign(int id)
        {
            try
            {
                var assignment = await GetAssignmentIdAsync(id);
                
                var developerWithTask = GetDeveloperWithTask(assignment);
                if (developerWithTask != null) { 
                db.Assignments.Attach(assignment);
                db.Entry(developerWithTask).Collection(x => x.Assignments).Load();
                db.Entry(assignment).Collection(x => x.Developers).Load();
                assignment.Developers.Remove(developerWithTask);
                developerWithTask.Assignments.Remove(assignment);
                assignment.AssingToNone = true;
                Update(assignment);
                await SaveChangesAsync();
                return;
                }
                assignment.Project.Tasks.Remove(assignment);
                assignment.Project = null;
                assignment.ProjectId = null;
                assignment.AssingToNone = true;
                Update(assignment);
                await SaveChangesAsync();
                return;
            }
            catch (Exception e)
            {

                throw e;
            }
            

        }
        public async Task<IEnumerable<AssignmentIndexViewModel>> IncludeAll()
        {
            var all = await GetAllIncludingAll();
            var data = all.Select(x => new AssignmentIndexViewModel
            {
                Id = x.Id,
                AssigmentName = x.Name,
                Deadline = x.Deadline,
                StatusName = x.Status.Name,
                Progress = x.Progress,
                ProjectName = x.Project == null ? "Unassigned" : x.Project.Name,
                AssignTo = x.AssingToNone == false ? x.Developers.Count > 0 ?
              x.Developers.Select(y => y.FullName).FirstOrDefault() + "(DEV)" : x.Project.ProjectManager.FullName :
              x.Project != null ? "Unasigned For DEV" : "Unasigned For PM"

            });
            return data;
        }

        public async Task<IEnumerable<AssignmentIndexViewModel>> IncludeDevelopersWithTasks()
        {
            var developersAssigment = db.Users.SelectMany(x => x.Assignments);
            var data = await developersAssigment.Select(x => new AssignmentIndexViewModel
            {
                Id = x.Id,
                AssigmentName = x.Name,
                Deadline = x.Deadline,
                StatusName = x.Status.Name,
                Progress = x.Progress,
                ProjectName = x.Project.Name,
                AssignTo = x.AssingToNone == false ? x.Developers.Select(y => y.FirstName + " " + y.LastName).FirstOrDefault() + "(DEV)" : "Unassigned"

            }).ToListAsync();
            return data;
        }
        public async Task<ApplicationUser> GetProjectManagerWithTask(int? projectId)
        {
            var projectManager = await db.Projects.Where(x => x.Id == projectId).Select(x => x.ProjectManager).FirstOrDefaultAsync();
            return projectManager;
        }
        private SelectListItem CreateNoneForDropdown()
        {
            SelectListItem none = new SelectListItem() { Value = "None", Text = "None", Selected = true };
            return none;

        }
        public async Task<Assignment> UpdateMapDataForDeveloper(AssignmentUpdateViewModel viewModel)
        {
            var assignment = await GetAssignmentIdAsync(viewModel.Id);
            //assignment.AssingToNone = viewModel.DeveloperId == "None" ? true : false;
            var project = await db.Projects.SingleOrDefaultAsync(x => x.Id == viewModel.ProjectId);
            var status = await db.Statuses.SingleOrDefaultAsync(x => x.Id == viewModel.StatusId);
            assignment.Deadline = viewModel.Deadline;
            assignment.Decription = viewModel.Description;
            var developer = assignment.AssingToNone == true ? await db.Users.SingleOrDefaultAsync(x => x.Id == viewModel.DeveloperId) :  GetDeveloperWithTask(assignment);
            assignment.Name = viewModel.AssigmentName;
            assignment.Progress = viewModel.Progress;
            assignment.Project = project;
            assignment.ProjectId = viewModel.ProjectId;
            assignment.Status = status;
            assignment.StatusId = viewModel.StatusId;
            assignment.StartDate = viewModel.StartDate;
            if (developer != null && assignment.AssingToNone==false)
            {
                try
                {
                   await UpdateAssigmentsAndDevelopers(viewModel.DeveloperId, assignment, developer);
                }
                catch (Exception e)
                {

                    throw e;
                }
                return assignment;
            }
            if(developer==null && viewModel.DeveloperId == "None")
            {
                return assignment;
            }
            assignment.Developers.Add(developer);
            developer.Assignments.Add(assignment);
            assignment.AssingToNone = false;
            return assignment;
        }


        public async Task<Assignment> UpdateMapDataForProjectManager(AssignmentUpdateViewModel viewModel)
        {
            var assignment = await GetAssignmentWithProjectAndProjectManagerAsync(viewModel.Id);
            var project =  await db.Projects.SingleOrDefaultAsync(x => x.Id == viewModel.ProjectId);
            var status = await db.Statuses.SingleOrDefaultAsync(x => x.Id == viewModel.StatusId);
            assignment.Deadline = viewModel.Deadline;
            assignment.Decription = viewModel.Description;
            var projectManager = assignment.AssingToNone == true ? await db.Users.SingleOrDefaultAsync(x => x.Id == viewModel.ProjectManagerId)
                : viewModel.ProjectId != assignment.ProjectId?
                await GetProjectManagerWithTask(viewModel.ProjectId):await GetProjectManagerWithTask(assignment.ProjectId);
            assignment.Name = viewModel.AssigmentName;
            assignment.Progress = viewModel.Progress;
            assignment.ProjectId = viewModel.ProjectId;
            assignment.Project = project;
            assignment.Status = status;
            assignment.StatusId = status.Id;
            if (projectManager != null && assignment.AssingToNone == false)
            {
                try
                {
                    project.ProjectManager = projectManager;
                }
                catch (Exception e)
                {

                    throw e;
                }
                return assignment;
            }

            assignment.Project.Tasks.Add(assignment);
            assignment.AssingToNone = false;
            return assignment;

        }

        public async Task<ApplicationUser> GetCurrentUserWithTasks(string id)
        {
           
            ApplicationUser currentUser = await db.Users.Include(x=>x.Assignments.Select(y=>y.Project)).Include(x=>x.Assignments.Select(y=>y.Status))
                .SingleOrDefaultAsync(x => x.Id == id);
            return currentUser;

        }

        public async Task<Assignment> AssingDeveloperToAssigment(AssignToDeveloperViewModel viewModel)
        {
            var assignment = await GetAssignmentIdAsync(viewModel.Id);
            var developer = GetDeveloperWithTask(assignment);
            if (developer != null)
            {
                try
                {
                   await UpdateAssigmentsAndDevelopers(viewModel.DeveloperId, assignment, developer);
                }
                catch (Exception e)
                {

                    throw e;
                }

            }
            return assignment;

        }

        private async Task UpdateAssigmentsAndDevelopers(string id, Assignment assignment,ApplicationUser developer)
        {
            db.Assignments.Attach(assignment);
            var newDeveloper = await db.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            db.Entry(newDeveloper).Collection(x => x.Assignments).Load();
            db.Entry(assignment).Collection(x => x.Developers).Load();
            assignment.Developers.Add(newDeveloper);
            assignment.Developers.Remove(developer);
            developer.Assignments.Remove(assignment);
            newDeveloper.Assignments.Add(assignment);
        }
            public void Nesto(int projectId)
        {
            throw new NotImplementedException();
        }
        public  IEnumerable<DeveloperCalendarViewModel> GetDeveloperCalendarDataAsync(ApplicationUser developer)
        {
            var tasks = developer.Assignments;
            var calendarData =  tasks.Select(x=> new DeveloperCalendarViewModel
            {
                AssigmentId = x.Id,
                AssigmentName = x.Name,
                Description = x.Decription,
                Start = x.StartDate,
                End = x.Deadline,
                Progress=x.Progress,
                StatusId = x.StatusId,
                ProjectName = x.Project.Name,
                StatusName = x.Status.Name

            }).ToList();
            return calendarData;
        }
        public async Task<Assignment> UpdateMapByDeveloperTask(ApplicationUser developer,DeveloperCalendarUpdateViewModel viewModel)
        {
            var assignment = developer.Assignments.SingleOrDefault(x => x.Id == viewModel.AssigmentId);
            var status = await db.Statuses.SingleOrDefaultAsync(x => x.Id == viewModel.StatusId);
            assignment.Progress = viewModel.Progress;
            assignment.Progress = viewModel.StatusId;
            assignment.Status = status;
            assignment.StatusId = viewModel.StatusId;
            return assignment;

        }
        //project manager = null project = null and assignToNone = true
        //project manager = "neko" project="nesto" assignToNone = false
        public async Task<List<string>> GetUnnasignedDeveloperTask()
        {
            var developerTask = db.Users.SelectMany(x => x.Assignments);
            var projectManagerTask = db.Assignments.SelectMany(x => x.Project.Tasks);
            var unassigned = await db.Assignments.Include(x=>x.Project.Tasks).Where(x => !developerTask.Select(y => y.Id).Contains(x.Id) &&
            !projectManagerTask.Select(y => y.Id).Contains(x.Id))
                .Select(x => x.Name).ToListAsync();
            return unassigned;

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