using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoTask.VievModels
{
    public class AssignmentUpdateViewModel
    {
        public int Id { get; set; }
        public string AssigmentName { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime StartDate { get; set; }
        public int StatusId { get; set; }
        public int? ProjectId { get; set; }
        public string DeveloperId { get; set; }
        public string ProjectManagerId { get; set; }
        public string Description { get; set; }
        [RegularExpression(@"^100(\.0{0,2})? *%?$|^\d{1,2}(\.\d{1,2})? *%?$", ErrorMessage = "From 0 to 100")]
        public double Progress { get; set; }

        public AssignmentUpdateViewModel()
        {

        }
        public AssignmentUpdateViewModel(int id,string assignmentName,DateTime startDate,DateTime deadline,
        int statusId,int? projectId,string description,double progress)
        {
            Id = id;
            AssigmentName = assignmentName;
            StartDate = startDate;
            Deadline = deadline;
            StatusId = statusId;
            ProjectId = projectId;
            Description = description;
            Progress = progress;

        }
    }
}