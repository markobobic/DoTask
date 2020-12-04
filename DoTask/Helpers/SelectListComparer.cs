using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoTask.Helpers
{
    public class SelectListComparer : IComparer<SelectListItem>
    {
    
        public int Compare(SelectListItem o1, SelectListItem o2)
        {
            if (o1.Equals(o2)) // update to make it stable
                return 0;
            if (o1.Text.Equals("None"))
                return -1;
            if (o2.Text.Equals("None"))
                return 1;
            return string.Compare(o1.Text, o2.Text);
        }
    }
}