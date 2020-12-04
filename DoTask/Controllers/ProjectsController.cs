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
    public class ProjectsController : Controller
    {

        private readonly IProjectService db;

        public ProjectsController(IProjectService _db)
        {
            db = _db;
        }
        // GET: Projects
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> Add()
        {
            ViewBag.ProjectManagers = await db.IncludeRoles();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(ProjectViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var project = await db.MapData(viewModel);
                db.CreateProject(project);
                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Added Successfully" });
            }
            ViewBag.ProjectManagers = db.IncludeRoles();
            return Index();
        }
        [HttpGet]
        public async Task<ActionResult> GetData()
        {
            var getAll = await db.GetAllProjectssWithProjectManager();
            var data = getAll.Select(x => new
            {
                Id=x.Id,
                Code = x.Code,
                Name = x.Name,
                ProjectManager = $"{x.ProjectManager.FirstName} {x.ProjectManager.LastName}"
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult>Update(int id)
        {
            if (id > 0)
            {
                var project = await db.GetProjectAndProjectManagertByIdAsync(id);
                if (project == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                }
                ProjectUpdateViewModel viewModel = new ProjectUpdateViewModel(project.Code,project.Name,
                 project.ProjectManagerId,id);
                ViewBag.ProjectManagers = await db.IncludeRoles();
                return View(viewModel);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(ProjectUpdateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var project = await db.UpdateMapData(viewModel);
                db.UpdateProject(project);
                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.ProjectManagers = await db.IncludeRoles();
            return new HttpStatusCodeResult(HttpStatusCode.NotModified);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var project = await db.GetUserWithDetailsAsync(id);
            project.Tasks.Clear();
            db.DeleteProject(project);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);

        }

    }
}