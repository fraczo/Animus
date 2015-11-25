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

        public static Array SelectItems(SPWeb web, string ct, string status, int minId=0)
        {
            SPList list = GetList(web);
            return list.Items.Cast<SPListItem>()
                .Where(i => i.ID>= minId) //filtruje rekordy utworzone po rekordzie inicjującym
                .Where(i => i.ContentType.Name.Equals(ct))
                //            & BLL.Tools.Get_Text(i, "enumStatusZlecenia").Equals(status))
                .ToArray();
        }
    }
}
