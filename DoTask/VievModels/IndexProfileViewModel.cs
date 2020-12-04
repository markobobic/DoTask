using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoTask.VievModels
{
    public class IndexProfileViewModel
    {
        public byte[] Photo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string PhotoType { get; set; }

        public IndexProfileViewModel(string firstName,string lastName,byte[] photo,string position,string photoType)
        {
            FirstName = firstName;
            LastName = lastName;
            Position = position;
            PhotoType = photoType;
            Photo = photo;
        }
    }
}