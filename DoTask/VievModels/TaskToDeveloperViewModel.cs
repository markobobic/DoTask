using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoTask.VievModels
{
    public class TaskToDeveloperViewModel
    {
        public string AssignmentName { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime Deadline { get; set; }
        public double Progress { get; set; }

        public string Description { get; set; }

        public int ProjectId { get; set; }
        public string DeveloperId { get; set; }
        public int StatusId { get; set; }



    }
}