using AutoMapper;
using DoTask.Models;
using DoTask.Services;
using DoTask.VievModels;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DoTask.Controllers
{
    public class UsersController : Controller
    {
        private readonly IMapper mapper;
        ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private readonly IUserService db;

        public UsersController(IUserService _db,IMapper _mapper)
        {
            db = _db;
            mapper = _mapper;
        }

        public ActionResult Index()
        {
            return View();
        }
        // GET: Users
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var data = await db.GetAllUsersWithRoles();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Delete(string id)
        {
            if(await db.AdminUserExist()) {
            var user = await UserManager.FindByIdAsync(id);
                try
                {
                    await db.DeleteProjectWithProjectManager(user);
                    var result = await UserManager.DeleteAsync(user);
                    if (result.Succeeded)
                        return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {

                    throw e;
                }
           
        }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        [HttpGet]
        public async Task<ActionResult> Update(string id)
        {

            var user = await db.GetUserByIdAsync(id);
            var userRole = await db.GetUserRole(id);
           
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            var userViewModel = mapper.Map<UserUpdateViewModel>(user);
            userViewModel.UserRole = userRole.Name;
            return View(userViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(UserUpdateViewModel userUpdate,HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(userUpdate.Id);
                var role = await db.GetUserRole(userUpdate.Id);
                var userToUpdate = db.MapData(user, userUpdate, image);
                if(userToUpdate.PasswordHash != user.PasswordHash) {
                    var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var resultForChangedPassword = await UserManager.ResetPasswordAsync(user.Id, token, userUpdate.PasswordHash);
                }
                var resultForUsers =  await UserManager.UpdateAsync(userToUpdate);
                if (userToUpdate.UserRole == RoleName.Admin || userToUpdate.UserRole == RoleName.Developer || userToUpdate.UserRole == RoleName.ProjectManager)
                {
                    string nesto = role.Name;
                    await UserManager.RemoveFromRoleAsync(userToUpdate.Id, role.Name);
                    await UserManager.AddToRoleAsync(userToUpdate.Id, userToUpdate.UserRole);

                }
                if (!resultForUsers.Succeeded)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotModified);
                }
                } else
                {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
                }

            return RedirectToAction("Index");

        }
    }
}