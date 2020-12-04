using DoTask.Models;
using DoTask.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoTask.Services
{
    public class StatusService
    {
        private readonly IGenericRepo<Status> _repo;

        public StatusService(IGenericRepo<Status> repo)
        {
            this._repo = repo;
        }
    }
}