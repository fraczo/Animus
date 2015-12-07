using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace BLL
{
    public class tabSzablonyWiadomosci
    {
        const string targetList = "Szablony wiadomości"; //"tabSzablonyWiadomosci";

        public static int GetSzablonId(Microsoft.SharePoint.SPWeb web, string nazwaSzablonu)
        {
            SPList list = web.Lists.TryGetList(targetList);

            SPListItem item = list.Items.Cast<SPListItem>()
                .Where(i => i.Title == nazwaSzablonu)
                .FirstOrDefault();

            if (item != null)
            {
                return item.ID;
            }

            return 0;
        }

        internal static void GetSzablonId(SPWeb web, int szablonId, out string subject, ref string bodyHTML, out string funkcjeSzablonu)
        {
            StringBuilder sb = new StringBuilder();

            SPList list = web.Lists.TryGetList(targetList);

            SPListItem item = list.GetItemById(szablonId);

            if (item != null)
            {
                funkcjeSzablonu = BLL.Tools.Get_Text(item, "colFunkcjeSzablonow");

                subject = BLL.Tools.Get_Text(item, "colTematWiadomosci");

                switch (item.ContentType.Name)
                {
                    case "Szablon wiadomości HTML":
                        sb = new StringBuilder(BLL.Tools.Get_Text(item, "colTrescHTML"));
                        break;
                    case "Szablon wiadomości":
                        sb = new StringBuilder(BLL.Tools.Get_Text(item, "colTresc"));
                        break;
                }

                sb.Append(bodyHTML);
            }
            else
            {
                funkcjeSzablonu = string.Empty;
                subject = string.Empty;
                sb.Append(bodyHTML);
            }

            bodyHTML = sb.ToString();
        }
    }
}
