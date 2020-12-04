using AutoMapper;
using DoTask.Models;
using DoTask.VievModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoTask.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {

            CreateMap<ApplicationUser, UserUpdateViewModel>();
            
        }
    }
}