using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoTask.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(15,ErrorMessage ="Maximum characters is 15")]
        [Index]
        public string Name { get; set; }
    }
}