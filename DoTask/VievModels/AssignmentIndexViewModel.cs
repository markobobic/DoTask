using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoTask.VievModels
{
    public class AssignmentIndexViewModel
    {
        public int Id { get; set; }
        public string AssigmentName { get; set; }
        public DateTime Deadline { get; set; }
        public string StatusName { get; set; }
        public double Progress { get; set; }
        public string ProjectName { get; set; }
        public string AssignTo { get; set; }
        //Id = x.Id,
        //        AssigmentName = x.Name,
        //        Deadline = x.Deadline,
        //        StatusName = x.Status.Name,
        //        Progress = x.Progress,
        //        ProjectName = x.Project.Name,
        //        AssingTo = x.AssingToNone
    }
}