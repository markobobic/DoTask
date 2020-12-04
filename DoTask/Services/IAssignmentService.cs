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
   public interface IAssignmentService
    {
        Task<IEnumerable<Assignment>> GetAllTasksAsync();
        Task<Assignment> GetAssignmentIdAsync(int assignmentId);
        Task<Assignment> GetAssignmentWithProjectAndUserAsync(int assignmentId);
        Task<IEnumerable<Assignment>> GetAllIncludingAll();
        Task SaveChangesAsync();
        Task<List<SelectListItem>> IncludeStatuses([Optional] int statusId);
        Task<List<SelectListItem>> IncludeProjects([Optional] int? projectId);
        Task<Tuple<SortedSet<SelectListItem>, SortedSet<SelectListItem>>> IncludeProjectManagersAndDevelopers();
        Task<dynamic> GetAllProjectManagers();
        Task<Assignment> MapData(AssignmentViewModel viewModel);
        Task<dynamic> GetProjectsWithProjectManagerId(string id);
        ApplicationUser GetDeveloperWithTask(Assignment assignment);
        Task<SortedSet<SelectListItem>> IncludeOnlyDevelopers(ApplicationUser developer);
        Task<ApplicationUser> GetProjectManagerWithTask(int? projectId);
        Task<SortedSet<SelectListItem>> IncludeOnlyProjectManagers(ApplicationUser projectM);
        Task<Assignment> UpdateMapDataForDeveloper(AssignmentUpdateViewModel viewModel);
        void CreateAssignment(Assignment assignment);
        void UpdateAssignment(Assignment assignment);
        void DeleteAssignment(Assignment assignment);
        void Nesto(int projectId);
    }
}
