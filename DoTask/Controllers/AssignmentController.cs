using DoTask.Models;
using DoTask.Services;
using DoTask.VievModels;
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
            var dropDownDevelopersProjects = await db.IncludeProjectManagersAndDevelopers();
            ViewBag.Statuses = await db.IncludeStatuses();
            ViewBag.Projects = await db.IncludeProjects();
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
            var dropDownDevelopersProjects = await db.IncludeProjectManagersAndDevelopers();
            ViewBag.Statuses = await db.IncludeStatuses();
            ViewBag.Projects = await db.IncludeProjects();
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
                        assignment.StatusId, assignment.ProjectId, assignment.Decription, assignment.Progress);
                    if (assignment.Developers.Count>0)
                    {
                        var developer = db.GetDeveloperWithTask(assignment);
                        ViewBag.Developers = await db.IncludeOnlyDevelopers(developer);
                        ViewBag.Projects = await db.IncludeProjects(viewModel.ProjectId);
                        ViewBag.Statuses = await db.IncludeStatuses(viewModel.StatusId);
                        return View(viewModel);
                    }
                    var projectManager = await db.GetProjectManagerWithTask(viewModel.ProjectId);
                    ViewBag.ProjectManagers = await db.IncludeOnlyProjectManagers(projectManager);
                    ViewBag.Projects = await db.IncludeProjects(viewModel.ProjectId);
                    ViewBag.Statuses = await db.IncludeStatuses(viewModel.StatusId);
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
                if (viewModel.DeveloperId !=null) { 
                var assignment = await db.UpdateMapDataForDeveloper(viewModel);
                db.UpdateAssignment(assignment);
                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }

            }
            
            return new HttpStatusCodeResult(HttpStatusCode.NotModified);
        }


        public async Task<ActionResult> DeveloperTasks()
        {
           
            return View();
        }

        public async Task<ActionResult> GetData()
        {
            var all = await db.GetAllIncludingAll();
            var data =  all.Select(x => new
            {
                Id = x.Id,
                AssigmentName = x.Name,
                Deadline = x.Deadline,
                StatusName = x.Status.Name,
                Progress = x.Progress,
                ProjectName = x.Project.Name,
                AssingTo = x.Developers.Count==0? x.Project.ProjectManager.FullName +"(PM)" 
                : x.Developers.Select(y=>y.FullName).FirstOrDefault() +"(DEV)"

            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public async Task<ActionResult> Description(int id)
        {
            var assignment = await db.GetAssignmentIdAsync(id);
            return View(assignment);

        }
        public async Task<JsonResult> GetProjectManager()
        {
            var projectManagers =await db.GetAllProjectManagers();
            return Json(projectManagers, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetProjects(string projectManagerId)
        {
            var projectsWithProjectManager =await db.GetProjectsWithProjectManagerId(projectManagerId);
            return Json(projectsWithProjectManager, JsonRequestBehavior.AllowGet);
        }
    }
}