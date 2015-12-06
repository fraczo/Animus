using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Collections;
using System.Diagnostics;

namespace BLL
{
    public static class tabStratyZLatUbieglych
    {
        private const string targetList = "Straty z lat ubiegłych";

        public static string Create_KEY(int klientId, int rok)
        {
            return string.Format(@"{0}::{1}", klientId.ToString(), rok.ToString());
        }

        public static Array Get_ExistingRecords(SPList list, int minYear, int maxYear)
        {
            return list.Items.Cast<SPListItem>()
                .Where(i => i["KEY"] != null)
                .Where(i => BLL.Tools.Get_Value(i, "colRokObrachunkowy") >= minYear
                            && BLL.Tools.Get_Value(i, "colRokObrachunkowy") <= maxYear)
                .ToArray();
        }


        public static SPList GetList(SPWeb web)
        {
            return web.Lists.TryGetList(targetList);
        }

        public static int AddNew(SPList list, int klientId, int rok, string key)
        {
            Debug.WriteLine("BLL.tabStratyZLatUbieglych.AddNew: " + key);

            SPListItem nItem = list.AddItem();
            BLL.Tools.Set_Index(nItem, "selKlient", klientId);
            BLL.Tools.Set_Index(nItem, "colRokObrachunkowy", rok);
            BLL.Tools.Set_Text(nItem, "KEY", key);

            nItem.Update();

            Debug.Write("-dodany");

            return nItem.ID;
        }
    }
}
