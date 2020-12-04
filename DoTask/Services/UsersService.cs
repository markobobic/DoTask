using DoTask.Models;
using DoTask.Repository;
using DoTask.VievModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DoTask.Services
{
    public class UsersService : GenericRepo<ApplicationUser>, IUserService
    {
       
        private readonly ApplicationDbContext db;
        public UsersService(ApplicationDbContext _db) : base(_db)
        {
            db = _db;
        }

        public void CreateUser(ApplicationUser user)
        {
            Create(user);
        }

        public void DeleteUser(ApplicationUser user)
        {
            Delete(user);
        }
        public async Task<IEnumerable<UserViewModel>> GetAllUsersWithRoles()
        {
            var data = await (from user in db.Users
                          from userRole in user.Roles
                          join role in db.Roles on userRole.RoleId equals role.Id
                          select new UserViewModel()
                          {
                              Id = user.Id,
                              Username = user.UserName,
                              FirstName = user.FirstName,
                              LastName = user.LastName,
                              Photo = user.Photo,
                              Email = user.Email,
                              UserRole = role.Name
                          }).ToListAsync();

              data.ForEach(user => user.PhotoPath = (user.Photo == null ? null : Convert.ToBase64String(user.Photo)));
              return data;

        }
        public async Task<IEnumerable<ApplicationUser>> GetAllUserssAsync()
        {
            return await FindAll().Include(role=>role.Roles)
            .OrderBy(us=>us.FirstName)
            .ToListAsync();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await FindByCondition(user => user.Id.Equals(userId))
           .FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> GetUserWithDetailsAsync(string userId)
        {
            return await FindByCondition(user => user.Id.Equals(userId))
              .Include(role => role.Roles)
              .FirstOrDefaultAsync();
        }

        public void UpdateUser(ApplicationUser user)
        {
            Update(user);
        }

        public async Task SaveChangesAsync()
        {
           await db.SaveChangesAsync();
        }

        public async Task<bool> AdminUserExist()
        {
           return await db.Roles.AnyAsync(x => x.Name == "Admin");
            
        }

        public async Task<IdentityRole> GetUserRole(string id)
        {
            var user =await GetUserByIdAsync(id);
            IdentityUserRole userRole = user.Roles.Where(x => x.UserId == id).FirstOrDefault();
            return await db.Roles.Where(x => x.Id == userRole.RoleId).SingleOrDefaultAsync();
          
        }

        public ApplicationUser MapData(ApplicationUser user, UserUpdateViewModel userUpdate, HttpPostedFileBase image)
        {
            if (image != null)
            {
                user.PhotoType = image.ContentType;
                user.Photo = new byte[image.ContentLength];
                image.InputStream.Read(user.Photo, 0, image.ContentLength);
            }
            user.Id = userUpdate.Id;
            user.FirstName = userUpdate.FirstName;
            user.LastName = userUpdate.LastName;
            user.Email = userUpdate.Email;
            user.UserRole = userUpdate.UserRole;
            return user;
        }

       
    }
}