using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Diagnostics;

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
            Debug.WriteLine("BLL.tabStawki.Ensure_KlientExist");
            Debug.WriteLine("klientId=" + klientId.ToString());

            if (klientId > 0)
            {
                SPList list = web.Lists.TryGetList(targetList);
                SPListItem item = list.Items.Cast<SPListItem>()
                    .Where(i => BLL.Tools.Get_LookupId(i, "selKlient") == klientId)
                    .FirstOrDefault();
                if (item == null)
                {
                    Debug.WriteLine(string.Format("klientId={0} nie istnieje w tabStawki", klientId.ToString()));

                    SPListItem newItem = list.AddItem();
                    newItem["selKlient"] = klientId;
                    
                    newItem.SystemUpdate();

                    Debug.WriteLine(string.Format("klientId={0} dodany do tabStawki", klientId.ToString()));
                }
            }
        }
    }
}
