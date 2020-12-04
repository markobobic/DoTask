using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoTask.VievModels
{
    public class ProjectViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ProjectManagerId { get; set; }
        public ProjectViewModel()
        {

        }
        public ProjectViewModel(string code,string name,string projectManagerId)
        {
            Code = code;
            Name = name;
            ProjectManagerId = projectManagerId;
        }
    }
}