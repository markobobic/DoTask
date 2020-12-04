using Autofac;
using Autofac.Integration.Mvc;
using DoTask.Helpers;
using DoTask.Models;
using DoTask.Repository;
using DoTask.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace DoTask.App_Start
{
    public class ContainerConfig
    {
        internal static void RegisterContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder
            .RegisterType<ApplicationDbContext>()
            .AsSelf().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(GenericRepo<>))
           .As(typeof(IGenericRepo<>)).InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(UsersService).Assembly)
           .Where(t => t.Name.EndsWith("Service"))
           .AsImplementedInterfaces()
           .InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(ProjectsService).Assembly)
           .Where(t => t.Name.EndsWith("Service"))
           .AsImplementedInterfaces()
           .InstancePerRequest();
             builder.RegisterAssemblyTypes(typeof(AssignmentService).Assembly)
           .Where(t => t.Name.EndsWith("Service"))
           .AsImplementedInterfaces()
           .InstancePerRequest();
            builder.RegisterModule<AutoFacAndAutoMapper>();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
         

        }

    }
}