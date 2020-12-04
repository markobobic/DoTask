using DoTask.Models;
using DoTask.VievModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DoTask.Services
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUserssAsync();
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<IdentityRole> GetUserRole(string id);
        Task<ApplicationUser> GetUserWithDetailsAsync(string userId);
        Task<IEnumerable<UserViewModel>> GetAllUsersWithRoles();
        Task SaveChangesAsync();
        Task<bool> AdminUserExist();
        void CreateUser(ApplicationUser user);
        void UpdateUser(ApplicationUser user);
        void DeleteUser(ApplicationUser user);
        ApplicationUser MapData(ApplicationUser user, UserUpdateViewModel userUpdate, HttpPostedFileBase image);

    }
}