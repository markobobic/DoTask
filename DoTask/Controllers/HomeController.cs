using DoTask.Models;
using DoTask.VievModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoTask.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public PartialViewResult _DashBoard()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().
            GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            IndexProfileViewModel viewModel = new IndexProfileViewModel
                (currentUser.FirstName, currentUser.LastName, currentUser.Photo, currentUser.UserRole, currentUser.PhotoType);
            return PartialView("~/Views/Shared/_DashBoard.cshtml", viewModel);

        }
    }
}