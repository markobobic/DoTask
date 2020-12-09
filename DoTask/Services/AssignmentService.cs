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
                .Include(x=>x.Project.ProjectManager)
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
            var currentProject =await db.Projects.AsNoTracking().SingleOrDefaultAsync(x => x.Id == projectId);
            var data = await Dropdown.GenerateDropdownProjectsForDEV(db,currentProject);
            return data;
        }
       
        public async Task<Assignment> MapData(AssignmentViewModel viewModel)
        {
            if (viewModel != null)
            {
                if (viewModel.DeveloperId != null)
                {
                    var assignment = new Assignment();
                    var project = await db.Projects.Include(x => x.ProjectManager).Include(x => x.Tasks).Where(x => x.Id == viewModel.ProjectId).FirstOrDefaultAsync();
                    var status = await db.Statuses.SingleOrDefaultAsync(x => x.Id == viewModel.StatusId);
                    assignment.Name = viewModel.AssigmentName;
                    assignment.Deadline = viewModel.Deadline;
                    assignment.Decription = viewModel.Description;
                    assignment.Progress = viewModel.Progress;
                    assignment.Status = status;
                    assignment.Project = project;
                    assignment.StartDate = viewModel.StartDate;
                    assignment.Project.ProjectManager = project.ProjectManager;
                    if (viewModel.DeveloperId == "None")
                    {
                        assignment.AssingToNone = true;
                        return assignment;
                    }
                    var developer = await db.Users.SingleOrDefaultAsync(x => x.Id == viewModel.DeveloperId);
                    assignment.Developers.Add(developer);
                    developer.Assignments.Add(assignment);
                    assignment.AssingToNone = false;
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
                    assignment.StartDate = viewModel.StartDate;
                    var project = await db.Projects.Where(x => x.Id == viewModel.ProjectId).Include(x => x.Tasks).Include(x => x.ProjectManager).FirstOrDefaultAsync();
                    assignment.Project = project;
                    assignment.Status = status;
                    assignment.AssingToNone = project == null || project.ProjectManager==null ?  true : false;
                    assignment.Project.ProjectManager = project.ProjectManager == null ? null : project.ProjectManager;
                    project.Tasks.Add(assignment);
                    return assignment;
                } 
            }
            return null;
        }

        public async Task<IEnumerable<CascadeDropdownProjectViewModel>> GetAllProjects()
        {
            return await db.Projects.Select(x => new CascadeDropdownProjectViewModel
            {
                ProjectId = x.Id,
                Name = x.Name,

            }).ToListAsync();

        }

        public async Task<List<CascadeDropdownProjectManager>> GetProjectManagersWithProjectId(int id)
        {
            try
            {
                return await db.Projects.Where(x => x.Id == id).Select(x => x.ProjectManager)
               .Select(x => new CascadeDropdownProjectManager
               {
                   Id =  x.Id == null? "None" : x.Id,
                   Name = x.FirstName + " " +x.LastName ==" "? "No one is assigned to selected project" : x.FirstName + " " + x.LastName
               }).ToListAsync();
            }
            catch (Exception e)
            {

                throw;
            }
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
            var developers = await Dropdown.GenereteDropDownUsers(db,RoleName.Developer);
            var none = Dropdown.CreateNoneForDropdown();
            developers.Add(none);
            var projectManagers = await Dropdown.GenereteDropDownUsers(db,RoleName.ProjectManager);
            Tuple<SortedSet<SelectListItem>, SortedSet<SelectListItem>> tuple = new Tuple<SortedSet<SelectListItem>, SortedSet<SelectListItem>>(developers, projectManagers);
            return tuple;
        }

        public async Task<SortedSet<SelectListItem>> IncludeDevelopersDropdown([Optional] bool isNone,[Optional]ApplicationUser developer)
        {
           
            SelectListItem currentDeveloper = isNone == false && developer!=null ? new SelectListItem() { Value = developer.Id, Text = developer.FullName, Selected = true }
                                                : new SelectListItem() { Value = "None", Text = "None", Selected = true };
           
            if (developer != null) {
                var developersForUpdate = await Dropdown.GenereteDropDownUsers(db,RoleName.Developer);
                developersForUpdate.Remove(developersForUpdate.Single(x => x.Value == developer.Id));
                developersForUpdate.Add(currentDeveloper);
              
                return developersForUpdate;
            }
            var developers = await Dropdown.GenereteDropDownUsers(db,RoleName.Developer);
            developers.Add(currentDeveloper);
          
            return developers;
        }

        public async Task<SortedSet<SelectListItem>> IncludeProjectManagersDropdown([Optional]ApplicationUser projectM)
        {
            if(projectM != null) { 
            SelectListItem currentProjectM = new SelectListItem() { Value = projectM.Id, Text = projectM.FullName, Selected = true };
            SelectListItem none = Dropdown.CreateNoneForDropdown();
            if (projectM != null) { 
                var projectManagersToUpdate = await Dropdown.GenereteDropDownUsers(db,RoleName.ProjectManager);
                projectManagersToUpdate.Remove(projectManagersToUpdate.Single(x => x.Value == projectM.Id));
                projectManagersToUpdate.Add(currentProjectM);
                projectManagersToUpdate.Add(none);
                return projectManagersToUpdate;

            }
            var projectManagers = await Dropdown.GenereteDropDownUsers(db,RoleName.ProjectManager);
            return projectManagers;
            }
            var projectManagersIfNone = await Dropdown.GenereteDropDownUsers(db,RoleName.ProjectManager);
            return projectManagersIfNone;
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
                db.Assignments.Attach(assignment);
                assignment.Project.Tasks.Remove(assignment);
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
            try
            {
                var data = all.Select(x => new AssignmentIndexViewModel
                {
                    Id = x.Id,
                    AssigmentName = x.Name,
                    Deadline = x.Deadline,
                    StatusName = x.Status.Name,
                    Progress = x.Progress,
                    ProjectName = x.ProjectId == null ? "Unassigned" : x.Project.Name,
                    AssignTo = (x.Developers.Count > 0) ? x.Developers.Select(y => y.FullName).FirstOrDefault() + "(DEV)" 
                    :(x.AssingToNone == true && x.ProjectId != null && x.Project.ProjectManager!=null) ? 
                    "Unnasigned DEV" : (x.Project==null || x.Project.ProjectManager==null)? 
                    "Unsigned" : x.Project.ProjectManager.FullName
                }).ToList();
                return data;
            }
            catch (Exception e)
            {

                throw;
            }
           
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
        
        public async Task<Assignment> UpdateMapDataForDeveloper(AssignmentUpdateViewModel viewModel)
        {
            var assignment = await db.Assignments.Include(x => x.Developers).
                Include(x => x.Project).SingleOrDefaultAsync(x => x.Id == viewModel.Id);
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
            if (developer != null && assignment.AssingToNone==false && viewModel.DeveloperId!="None")
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
            if (viewModel.DeveloperId == "None")
            {
                if (developer != null)
                {
                    assignment.Developers.Remove(developer);
                    developer.Assignments.Remove(assignment);
                }
                assignment.AssingToNone = true;
                return assignment;
            }
            assignment.Developers.Add(developer);
            developer.Assignments.Add(assignment);
            assignment.AssingToNone = false;
            return assignment;
        }


        public async Task<Assignment> UpdateMapDataForProjectManager(AssignmentUpdateViewModel viewModel)
        {
            var assignment =await db.Assignments.Include(x => x.Project).Include(x => x.Project.ProjectManager).Include(x => x.Project.Tasks)
                .SingleAsync(x => x.Id == viewModel.Id);
            var project =  await db.Projects.SingleOrDefaultAsync(x => x.Id == viewModel.ProjectId);
            var status = await db.Statuses.SingleOrDefaultAsync(x => x.Id == viewModel.StatusId);
            assignment.Deadline = viewModel.Deadline;
            assignment.Decription = viewModel.Description;
            var projectManager = viewModel.ProjectManagerId == "None" ? null : project.ProjectManager;
            assignment.AssingToNone = viewModel.ProjectManagerId == "None" ? true : false;
            assignment.Name = viewModel.AssigmentName;
            assignment.Progress = viewModel.Progress;
            assignment.ProjectId = viewModel.ProjectId;
            assignment.Project = project;
            assignment.Status = status;
            assignment.StatusId = status.Id;
            assignment.Project.Tasks.Add(assignment);
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

        public async Task<IEnumerable<ProjectManagerCalendarViewModel>> GetProjectManagerCalendarDataAsync(ApplicationUser projectM)
        {

            var tasks = await db.Assignments.Include(x=>x.Status).Where(x => x.Developers.Count == 0 
            && x.AssingToNone == false && x.Project.ProjectManagerId==projectM.Id).ToListAsync();
            var calendarData = tasks.Select(x => new ProjectManagerCalendarViewModel
            {
                AssigmentId = x.Id,
                AssigmentName = x.Name,
                Description = x.Decription,
                Start = x.StartDate,
                End = x.Deadline,
                Progress = x.Progress,
                StatusId = x.StatusId,
                ProjectName = x.Project.Name,
                StatusName = x.Status.Name,
                
                

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
        public async Task<Assignment> UpdateMapByProjectManagerTask(ProjectManagerCalendarViewModel viewModel)
        {
            var assignment = db.Assignments.SingleOrDefault(x => x.Id == viewModel.AssigmentId);
            var status = await db.Statuses.SingleOrDefaultAsync(x => x.Id == viewModel.StatusId);
            assignment.Progress = viewModel.Progress;
            assignment.Progress = viewModel.StatusId;
            assignment.Status = status;
            assignment.StatusId = viewModel.StatusId;
            assignment.Deadline = viewModel.End;
            return assignment;

        }


        public async Task<Assignment> UpdateMapByProjectManerDeveloperTask(ProjectTaskCalendarUpdateViewModel viewModel)
        {
            var assignment = db.Assignments.Include(x=>x.Developers).Include(x => x.Project)
                .SingleOrDefault(x => x.Id == viewModel.AssigmentId);
            var developer = assignment.AssingToNone == true ? await db.Users.SingleOrDefaultAsync(x => x.Id == viewModel.DeveloperId) : GetDeveloperWithTask(assignment);
            var status = await db.Statuses.SingleOrDefaultAsync(x => x.Id == viewModel.StatusId);
            assignment.Progress = viewModel.Progress;
            assignment.Progress = viewModel.StatusId;
            assignment.Status = status;
            assignment.StatusId = viewModel.StatusId;
            assignment.Deadline = viewModel.End;
            if (developer != null && assignment.AssingToNone == false && viewModel.DeveloperId != "None")
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
            if (viewModel.DeveloperId=="None")
            {
                if(developer != null)
                {
                    assignment.Developers.Remove(developer);
                    developer.Assignments.Remove(assignment);
                }
                assignment.AssingToNone = true;
                return assignment;
            }
            assignment.Developers.Add(developer);
            developer.Assignments.Add(assignment);
            assignment.AssingToNone = false;
            return assignment;

        }


        public async Task<IEnumerable<ProjectTaskCalendarIndexViewModel>> GetProjectTasksDevelopersDataAsync(ApplicationUser projectM)
        {
            var tasks = db.Projects.Where(x => x.ProjectManagerId == projectM.Id).SelectMany(x => x.Tasks);
            var calendarData = await tasks.Where(x=>x.Developers.Count>0 || (x.AssingToNone==true && x.ProjectId!=null)).Select(x => new ProjectTaskCalendarIndexViewModel
            {
                AssigmentId = x.Id,
                AssigmentName = x.Name,
                Description = x.Decription,
                Start = x.StartDate,
                End = x.Deadline,
                Progress = x.Progress,
                StatusId = x.StatusId,
                ProjectName = x.Project.Name,
                StatusName = x.Status.Name,
                AssignTo = x.Developers.Count > 0? x.Developers.Select(y=>y.FirstName+ " " + y.LastName).FirstOrDefault() 
                : "None",
                DeveloperId = x.Developers.Count>0? x.Developers.Select(y=>y.Id).FirstOrDefault() : "None"

            }).ToListAsync();
            return calendarData;
        }




        public async Task<List<UnassignedDeveloperViewModel>> GetUnnasignedDeveloperTask()
        {

            var unassigned = await db.Assignments.Include(x => x.Project.ProjectManager).Include(x => x.Developers).Where(assignment =>
            assignment.AssingToNone == true && assignment.ProjectId
            != null && assignment.Project.ProjectManagerId != null).Select(x => new UnassignedDeveloperViewModel
            {
                AssigmentName = x.Name,
                Deadline = x.Deadline,
                StatusName = x.Status.Name,
                Progress = x.Progress,
                ProjectName = x.Project.Name,
                AssignTo = "None"
            }).ToListAsync();

            return unassigned;
        }


        public async Task<List<UnassignedProjectManagerViewModel>> GetUnnasignedProjectManagers()
        {

            var unassigned = await db.Assignments.Where(x=>x.ProjectId==null).Select(x => new UnassignedProjectManagerViewModel
                {
                AssigmentName = x.Name,
                Deadline = x.Deadline,
                StatusName = x.Status.Name,
                Progress = x.Progress,
                ProjectName = "None",
                AssignTo = "None"
            }).ToListAsync();

            return unassigned;
        }
        public async Task<List<SelectListItem>> IncludeManagedProjectsByProjectM(string id)
        {
            List<SelectListItem> data = new List<SelectListItem>();

            data = await db.Projects.Where(x=>x.ProjectManagerId==id).Select(proj => new SelectListItem()
            {
                Value = proj.Id.ToString(),
                Text = proj.Name
            }).OrderBy(x=>x.Text).ToListAsync();

            if (data.Count == 0)
            {
                SelectListItem projectManagerWithoutProject = new SelectListItem() { Text = "Project manager isn't assigned to any project",
                    Value = "None", Selected = true };
                data.Add(projectManagerWithoutProject);
                return data;
            }
            return data;

        }

        public async Task<ApplicationUser> GetCurrentUser()
        {
            return await System.Web.HttpContext.Current.GetOwinContext().
               GetUserManager<ApplicationUserManager>().FindByIdAsync(System.Web.HttpContext.Current.User.Identity.GetUserId());
        }


    }
}