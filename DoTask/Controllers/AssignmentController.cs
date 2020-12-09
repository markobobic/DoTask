using DoTask.Helpers;
using DoTask.Models;
using DoTask.Services;
using DoTask.VievModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DoTask.Controllers
{
    public class AssignmentController : Controller
    {
        // GET: Tasks
        private readonly IAssignmentService db;

        public AssignmentController(IAssignmentService _db)
        {
            db = _db;
        }


        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> Add()
        {
            var dropDownDevelopersProjects = await db.IncludeProjectManagersAndDevelopersDropdown();
            ViewBag.Statuses = await db.IncludeStatusesDropdown();
            ViewBag.Projects = await db.IncludeProjectsDropdown();
            ViewBag.Developers = dropDownDevelopersProjects.Item1;
            ViewBag.ProjectManagers = dropDownDevelopersProjects.Item2;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AssignmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var assigment = await db.MapData(viewModel);
                db.CreateAssignment(assigment);
                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Added Successfully" });
            }
            var dropDownDevelopersProjects = await db.IncludeProjectManagersAndDevelopersDropdown();
            ViewBag.Statuses = await db.IncludeStatusesDropdown();
            ViewBag.Projects = await db.IncludeProjectsDropdown();
            ViewBag.Developers = dropDownDevelopersProjects.Item1;
            ViewBag.ProjectManagers = dropDownDevelopersProjects.Item2;
            return Index();
        }


        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            if (id > 0)
            {
                var assignment = await db.GetAssignmentIdAsync(id);

                if (assignment != null)
                {
                    AssignmentUpdateViewModel viewModel = new AssignmentUpdateViewModel(assignment.Id, assignment.Name, assignment.Deadline,
                       assignment.StartDate, assignment.StatusId, assignment.ProjectId, assignment.Decription, assignment.Progress);
                    if ((assignment.Developers.Count>0) || (assignment.AssingToNone==true && assignment.ProjectId!=null && assignment.Project.ProjectManagerId!=null))
                    {
                        var developer = db.GetDeveloperWithTask(assignment);
                        ViewBag.Developers = await db.IncludeDevelopersDropdown(assignment.AssingToNone, developer);
                        ViewBag.Projects = await db.IncludeProjectsDropdown(viewModel.ProjectId);
                        ViewBag.Statuses = await db.IncludeStatusesDropdown(viewModel.StatusId);
                        return View(viewModel);
                    }
                    var projectManager = await db.GetProjectManagerWithTask(viewModel.ProjectId);
                    ViewBag.Statuses = await db.IncludeStatusesDropdown(viewModel.StatusId);
                    viewModel.ProjectManagerId = projectManager == null ? "None" : projectManager.Id;
                    return View(viewModel);
                }

            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(AssignmentUpdateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.DeveloperId != null)
                {
                    var assignmentForDeveloper = await db.UpdateMapDataForDeveloper(viewModel);
                    db.UpdateAssignment(assignmentForDeveloper);
                    await db.SaveChangesAsync();
                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                var assignmentForProjectManager = await db.UpdateMapDataForProjectManager(viewModel);
                db.UpdateAssignment(assignmentForProjectManager);
                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);

            }

            return new HttpStatusCodeResult(HttpStatusCode.NotModified);
        }

        public async Task<ActionResult> GetData()
        {

            if (User.IsInRole(RoleName.Admin))
            {

                var data = await db.IncludeAll();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if (User.IsInRole(RoleName.ProjectManager))
            {
                var data = await db.IncludeDevelopersWithTasks();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        }

        [HttpGet]
        public async Task<ActionResult> AssignDeveloper(int id)
        {

            var assignment = await db.GetAssignmentIdAsync(id);
            var developer = db.GetDeveloperWithTask(assignment);
            ViewBag.Developers = await db.IncludeDevelopersDropdown(assignment.AssingToNone, developer);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignDeveloper(AssignToDeveloperViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.DeveloperId != null)
                {
                    var assignment = await db.AssingDeveloperToAssigment(viewModel);
                    db.UpdateAssignment(assignment);
                    await db.SaveChangesAsync();
                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }

            }

            return new HttpStatusCodeResult(HttpStatusCode.NotModified);
        }


        [HttpGet]
        public async Task<ActionResult> Description(int id)
        {
            var assignment = await db.GetAssignmentIdAsync(id);
            return View(assignment);

        }

        public async Task<ActionResult> GetProjects()
        {
            var projects = await db.GetAllProjects();
            return Json(projects, JsonRequestBehavior.AllowGet);

           
        }

        public async Task<JsonResult> GetProjectManager(int projectId)
        {
            var projectsWithProjectManager = await db.GetProjectManagersWithProjectId(projectId);
            if (projectsWithProjectManager.Count == 0 )
            {
                var dropdown = await db.IncludeProjectManagersDropdown();
                var data = dropdown.Select(x => new CascadeDropdownProjectViewModel { ProjectId = x.Value.ToInt32(), Name = x.Text });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(projectsWithProjectManager, JsonRequestBehavior.AllowGet);
        }
        

        [HttpPost]
        public async Task<ActionResult> Unassign(int id)
        {
            await db.Unassign(id);

            return Json(new { success = true, message = "Unassigned Successfully" }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public async Task<ActionResult> ProjectManagerTasks()
        {

            ViewBag.Statuses = await db.IncludeStatusesDropdown();
            return View();
        }

        public async Task<JsonResult> GetProjectManagerTasks()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = await db.GetCurrentUserWithTasks(currentUserId);
            var calendarData =await db.GetProjectManagerCalendarDataAsync(currentUser);
            return new JsonResult { Data = calendarData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public async Task<ActionResult> UpdateByProjectManagerTask(ProjectManagerCalendarViewModel viewModel)
        {
            var updatedAssigment = await db.UpdateMapByProjectManagerTask(viewModel);
            db.UpdateAssignment(updatedAssigment);
            await db.SaveChangesAsync();
            return new JsonResult { Data = new { status = "save" } };

        }

        [HttpGet]
        public async Task<ActionResult> ProjectsTaskDevelopers()
        {

            ViewBag.Statuses = await db.IncludeStatusesDropdown();
            ViewBag.Developers = await db.IncludeDevelopersDropdown();
            return View();
        }

        public async Task<JsonResult> GetProjectTasksDevelopers()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = await db.GetCurrentUserWithTasks(currentUserId);
            var calendarData = await db.GetProjectTasksDevelopersDataAsync(currentUser);
            return new JsonResult { Data = calendarData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public async Task<ActionResult> UpdateByProjectTasksDevelopers(ProjectTaskCalendarUpdateViewModel viewModel)
        {
            var updatedAssigment = await db.UpdateMapByProjectManerDeveloperTask(viewModel);
            db.UpdateAssignment(updatedAssigment);
            await db.SaveChangesAsync();
            return new JsonResult { Data = new { status = "save" } };

        }


        [HttpGet]
        public async Task<ActionResult> DeveloperTasks()
        {

            ViewBag.Statuses = await db.IncludeStatusesDropdown();
            return View();
        }

        public async Task<JsonResult> GetDevTasks()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = await db.GetCurrentUserWithTasks(currentUserId);
            var calendarData = db.GetDeveloperCalendarDataAsync(currentUser);
            return new JsonResult { Data = calendarData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public async Task<ActionResult> UpdateByDeveloperTask(DeveloperCalendarUpdateViewModel viewModel)
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = await db.GetCurrentUserWithTasks(currentUserId);
            var updatedAssigment = await db.UpdateMapByDeveloperTask(currentUser, viewModel);
            db.UpdateAssignment(updatedAssigment);
            await db.SaveChangesAsync();
            return new JsonResult { Data = new { status = "save" } };

        }


        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var assignment = await db.GetAssignmentIdAsync(id);
            db.DeleteAssignment(assignment);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult UnassignedDevelopers()
        {
            return View();
        }
        [HttpGet]
        public ActionResult UnassignedProjectManagers()
        {
            return View();
        }

        public async Task<ActionResult> GetUnassignedData()
        {
            if (User.IsInRole(RoleName.Developer)) { 
            var data = await db.GetUnnasignedDeveloperTask();
            return Json(data, JsonRequestBehavior.AllowGet);
            }
            if (User.IsInRole(RoleName.ProjectManager))
            {
                var data = await db.GetUnnasignedProjectManagers();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }


        public async Task<ActionResult> AddToDeveloper()
        {
           
            ViewBag.Statuses = await db.IncludeStatusesDropdown();
            ViewBag.Developers = await db.IncludeDevelopersDropdown();
            var currentProjectManager = await db.GetCurrentUser();
            ViewBag.Projects = await db.IncludeManagedProjectsByProjectM(currentProjectManager.Id);
            ViewBag.ProjectManagerId = currentProjectManager.Id;
            ViewBag.ProjectManagerName = currentProjectManager.FullName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddToDeveloper(AssignmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var assigment = await db.MapData(viewModel);
                db.CreateAssignment(assigment);
                await db.SaveChangesAsync();
                RedirectToAction("ProjectsTaskDevelopers");

            }
            ViewBag.Statuses = await db.IncludeStatusesDropdown();
            ViewBag.Developers = await db.IncludeDevelopersDropdown();
            var currentProjectManager = await db.GetCurrentUser();
            ViewBag.Projects = await db.IncludeManagedProjectsByProjectM(currentProjectManager.Id);
            ViewBag.ProjectManagerId = currentProjectManager.Id;
            ViewBag.ProjectManagerName = currentProjectManager.FullName;
            return RedirectToAction("ProjectsTaskDevelopers");
        }



    }
}