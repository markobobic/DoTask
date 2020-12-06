using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoTask.VievModels
{
    public class DeveloperCalendarViewModel
    {
        public int AssigmentId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }
        public string Description { get; set; }
        public int StatusId { get; set; }
        public string ProjectName { get; set; }
        public double Progress { get; set; }
        public string AssigmentName { get; set; }
        public string StatusName { get; set; }
    }
}