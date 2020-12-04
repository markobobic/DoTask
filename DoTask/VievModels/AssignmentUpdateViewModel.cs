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
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Deadline { get; set; }
        public int StatusId { get; set; }
        public int? ProjectId { get; set; }
        public string DeveloperId { get; set; }
        public string ProjectManagerId { get; set; }
        public string Description { get; set; }
        public double Progress { get; set; }

        public AssignmentUpdateViewModel()
        {

        }
        public AssignmentUpdateViewModel(int id,string assignmentName,DateTime deadline,
        int statusId,int? projectId,string description,double progress)
        {
            Id = id;
            AssigmentName = assignmentName;
            Deadline = deadline;
            StatusId = statusId;
            ProjectId = projectId;
            Description = description;
            Progress = progress;

        }
    }
}