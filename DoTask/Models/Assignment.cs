using DoTask.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoTask.Models
{
    public class Assignment
    {
        [Key]
        public int Id { get; set; }
        public double Progress { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Deadline { get; set; }
        [StringLength(maximumLength: 20, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 20 characters!")]
        [Required(ErrorMessage = "You must provide task name")]
        public string Name { get; set; }
        [StringLength(maximumLength: 250, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 250 characters!")]
        public string Decription { get; set; }
        public bool AssingToNone { get; set; }
        public Status Status { get; set; }
        public int StatusId { get; set; }
        public int? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
       
        public ICollection<ApplicationUser> Developers { get; set; } = new List<ApplicationUser>();
        

    }
}