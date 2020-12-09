using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoTask.VievModels
{
    public class AssignmentViewModel
    {
        public string AssigmentName { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime StartDate { get; set; }
        public int StatusId { get; set; }
        public int ProjectId { get; set; }
        public string DeveloperId { get; set; }
        public string ProjectManagerId { get; set; }
        public string Description { get; set; }
        public double Progress { get; set; }

    }
}