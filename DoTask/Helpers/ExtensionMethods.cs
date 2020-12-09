using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoTask.Helpers
{
    public static class ExtensionMethods
    {
        public static Int32 ToInt32(this object obj)
        {
            return Convert.ToInt32(obj);
        }
    }
}