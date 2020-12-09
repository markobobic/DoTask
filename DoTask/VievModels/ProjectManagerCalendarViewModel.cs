using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoTask.VievModels
{
    public class ProjectManagerCalendarViewModel
    {
        public int AssigmentId { get; set; }
        public DateTime Start { get; set; }
        private DateTime end;

        public DateTime End
        {
            get { return end.AddDays(1); }
            set { end = value; }
        }
        public string Description { get; set; }
        public int StatusId { get; set; }
        public string ProjectName { get; set; }
        public double Progress { get; set; }
        public string AssigmentName { get; set; }
        public string StatusName { get; set; }

      
    }
}