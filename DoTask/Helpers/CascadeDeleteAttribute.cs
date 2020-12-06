using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoTask.Helpers
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CascadeDeleteAttribute : Attribute { }
}