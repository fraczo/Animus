using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace BLL
{
    public class tabStawki
    {
        const string targetList = @"Stawki";

        internal static double Get_OplataMiesieczna(Microsoft.SharePoint.SPWeb web, int klientId)
        {
            SPList list = web.Lists.TryGetList(targetList);
            SPListItem item = list.Items.Cast<SPListItem>()
                .Where(i => BLL.Tools.Get_LookupId(i, "selKlient").Equals(klientId))
                .FirstOrDefault();

            if (item != null)
            {
                return BLL.Tools.Get_Double(item, "colOplataMiesieczna");
            }
            else
            {
                return 0;
            }
        }

        public static void Ensure_KlientExist(SPWeb web, int klientId)
        {
            SPList list = GetList(web);
            SPListItem item = list.Items.Cast<SPListItem>()
                .Where(i => BLL.Tools.Get_LookupId(i, "selKlient").Equals(klientId))
                .FirstOrDefault();
            if (item==null)
            {
                SPListItem newItem = list.AddItem();
                newItem["selKlient"] = klientId;
                newItem.SystemUpdate();
            }
        }

        private static SPList GetList(SPWeb web)
        {
            return web.Lists.TryGetList(targetList);
        }

    }
}
