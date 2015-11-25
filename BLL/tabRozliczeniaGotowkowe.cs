using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace BLL
{
    /// <summary>
    /// Run with elevated privilages
    /// </summary>
    public class tabRozliczeniaGotowkowe
    {
        public const string targetList = @"Rejestr płatności";

        public static string Define_KEY(int klientId, int okresId)
        {
            return String.Format(@"{0}:{1}",
                klientId.ToString(),
                okresId.ToString());
        }

        public static bool Check_KEY_IsAllowed(string key, Microsoft.SharePoint.SPWeb web, int currentId)
        {
            bool result = true;

            var list = web.Lists.TryGetList(targetList);

            Array li = list.Items.Cast<SPListItem>()
                    .Where(i => i.ID != currentId)
                    .Where(i => BLL.Tools.Get_Text(i, "KEY").Equals(key))
                    .ToArray();

            if (li.Length > 0)
            {
                result = false;
            }

            return result;
        }

        public static void Create_ctRB_Form(SPWeb web, int klientId, int okresId, string key, SPListItem klientItem, Models.Klient iok)
        {
            Logger.LogEvent("Create_ctRB_Form", klientId.ToString());

            SPList list = web.Lists.TryGetList(targetList);
            SPListItem newItem = list.AddItem();

            BLL.Tools.Set_Value(newItem, "selKlient", klientId);
            BLL.Tools.Set_Value(newItem, "selOkres", okresId);
            BLL.Tools.Set_Text(newItem, "KEY", key);

            Models.Okres o = new Models.Okres(web, okresId);

            BLL.Tools.Set_Text(newItem, "Title", string.Format("Opłata za obsługę {0}", o.Nazwa));

            double om = BLL.tabStawki.Get_OplataMiesieczna(web, klientId);
            if (om > 0)
            {
                BLL.Tools.Set_Value(newItem, "colDoZaplaty", om);
            }
            else
            {
                BLL.tabStawki.Ensure_KlientExist(web, klientId);
            }

            newItem.SystemUpdate();
        }
    }
}
