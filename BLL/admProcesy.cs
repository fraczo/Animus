using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace BLL
{
    public class admProcesy
    {
        const string targetList = "admProcesy";

        public static SPList GetList(SPWeb web)
        {
            return web.Lists.TryGetList(targetList);
        }
    }
}
