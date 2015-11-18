using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace EventReceivers.admProcesy
{
    class GFR_Request
    {
        internal static void Create(Microsoft.SharePoint.SPListItem item)
        {
            string mask = "ZUS-*";
            Array klienci = BLL.tabKlienci.Get_AktywniKlienci_BySerwisMask(item.Web, mask);

            SPList list = BLL.admProcesy.GetList(item.Web);

            foreach (SPListItem k in klienci)
            {
                string ct = "Generowanie formatek rozliczeniowych dla klienta";
                int okresId = BLL.Tools.Get_LookupId(item, "selOkres");

                SPListItem newItem = list.AddItem();
                newItem["ContentType"] = ct;
                newItem["selKlient"] = k.ID;
                newItem["selOkres"] = okresId;
                newItem.SystemUpdate();

            }
        }
    }
}
