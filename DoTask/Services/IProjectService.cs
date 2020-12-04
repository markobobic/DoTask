using DoTask.Models;
using DoTask.VievModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DoTask.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project> GetProjectAndProjectManagertByIdAsync(int projectId);
        Task<Project> GetUserWithDetailsAsync(int projectId);
        Task<IEnumerable<Project>> GetAllProjectssWithTasks();
        Task<IEnumerable<Project>> GetAllProjectssWithProjectManager();
        Task<SelectList> IncludeRoles();
        Task SaveChangesAsync();
        void CreateProject(Project project);
        void UpdateProject(Project project);
        void DeleteProject(Project project);
        Task<Project> MapData(ProjectViewModel viewModel);
        Task<Project> UpdateMapData(ProjectUpdateViewModel updateViewModel);
    }
}
