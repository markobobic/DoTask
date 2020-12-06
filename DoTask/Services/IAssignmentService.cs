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
        Task<Assignment> GetAssignmentWithProjectAndProjectManagerAsync(int assignmentId);
        Task<IEnumerable<Assignment>> GetAllIncludingAll();
        Task SaveChangesAsync();
        Task<List<SelectListItem>> IncludeStatusesDropdown([Optional] int statusId);
        Task<List<SelectListItem>> IncludeProjectsDropdown([Optional] int? projectId);
        Task<Tuple<SortedSet<SelectListItem>, SortedSet<SelectListItem>>> IncludeProjectManagersAndDevelopersDropdown();
        Task<dynamic> GetAllProjectManagers();
        Task<Assignment> MapData(AssignmentViewModel viewModel);
        Task<List<CascadeDropdownProjectViewModel>> GetProjectsWithProjectManagerId(string id);
        ApplicationUser GetDeveloperWithTask(Assignment assignment);
        Task<SortedSet<SelectListItem>> IncludeDevelopersDropdown([Optional] bool isNone, [Optional] ApplicationUser developer);
        Task<ApplicationUser> GetCurrentUserWithTasks(string id);
        IEnumerable<DeveloperCalendarViewModel> GetDeveloperCalendarDataAsync(ApplicationUser developer);
        Task<ApplicationUser> GetProjectManagerWithTask(int? projectId);
        Task<Assignment> UpdateMapByDeveloperTask(ApplicationUser developer, DeveloperCalendarUpdateViewModel viewModel);
        Task<List<string>> GetUnnasignedDeveloperTask();
        Task<SortedSet<SelectListItem>> IncludeProjectManagersDropdown(ApplicationUser projectM);
        Task<Assignment> AssingDeveloperToAssigment(AssignToDeveloperViewModel viewModel);
        Task<Assignment> GetAssignmentWithDevelopers(int assignmentId);
        Task<Assignment> UpdateMapDataForDeveloper(AssignmentUpdateViewModel viewModel);
        Task<IEnumerable<AssignmentIndexViewModel>> IncludeDevelopersWithTasks();
        Task<IEnumerable<AssignmentIndexViewModel>> IncludeAll();
        Task<Assignment> UpdateMapDataForProjectManager(AssignmentUpdateViewModel viewModel);
        Task Unassign(int id);
        void CreateAssignment(Assignment assignment);
        void UpdateAssignment(Assignment assignment);
        void DeleteAssignment(Assignment assignment);
        void Nesto(int projectId);
    }
}
