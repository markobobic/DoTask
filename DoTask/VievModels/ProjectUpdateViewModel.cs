using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoTask.VievModels
{
    public class ProjectUpdateViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ProjectManagerId { get; set; }
        public ProjectUpdateViewModel()
        {

        }
        public ProjectUpdateViewModel(string code, string name, string projectManagerId,int id)
        {
            Code = code;
            Name = name;
            ProjectManagerId = projectManagerId;
            Id = id;
        }
    }
}