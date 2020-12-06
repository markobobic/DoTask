using DoTask.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoTask.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        [StringLength(maximumLength: 50, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 50 characters!")]
        [Required(ErrorMessage = "You must provide name of project")]
        public string Name { get; set; }
        [Required(ErrorMessage = "You must provide project code")]
        [Remote("DoesCodeExist", "Projects", ErrorMessage = "Code already exists")]
        [StringLength(maximumLength: 50, MinimumLength = 1, ErrorMessage = "Code must be between 1 and 50 characters!")]
        public string Code { get; set; }
        public ICollection<Assignment> Tasks { get; set; } = new List<Assignment>();
        [Required]
        public string ProjectManagerId { get; set; }
        
        public ApplicationUser ProjectManager { get; set; }


    }
}