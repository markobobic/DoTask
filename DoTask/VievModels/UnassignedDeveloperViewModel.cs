using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoTask.VievModels
{
    public class UnassignedDeveloperViewModel
    {
        public string AssigmentName { get; set; }
        public string StatusName { get; set; }

        public string ProjectName { get; set; }
        public double Progress { get; set; }

        public DateTime Deadline { get; set; }
        public string AssignTo { get; set; }
    }
}