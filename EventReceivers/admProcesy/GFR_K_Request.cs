using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace EventReceivers.admProcesy
{
    class GFR_K_Request
    {
        internal static void Create(Microsoft.SharePoint.SPListItem item)
        {
            int okresId = BLL.Tools.Get_LookupId(item, "selOkres");
            int klientId = BLL.Tools.Get_LookupId(item, "selKlient");
            string mask = BLL.Tools.Get_Text(item, "colMaskaSerwisu");

            if (okresId > 0 && klientId > 0)
            {
                BLL.Models.Klient iok = new BLL.Models.Klient(item.Web, klientId);

                SPListItem klientItem = BLL.tabKlienci.Get_KlientById(item.Web, klientId);

                switch (iok.TypKlienta)
                {
                    case "KPiR":
                        if (mask.StartsWith("ZUS-") || string.IsNullOrEmpty(mask))
                            ZUS_Forms.Manage_ZUS_Form(item.Web, okresId, klientItem, iok);
                        if (mask.StartsWith("PD-") || string.IsNullOrEmpty(mask))
                            PD_Forms.Create_PD_Form(item.Web, okresId, klientItem, iok);
                        //VAT_Forms.Create(item.Web, klientId, okresId);
                        break;
                    case "KSH":
                        //ZUS_Forms.Create(item.Web, klientId, okresId);
                        //PDS_Forms.Create(item.Web, klientId, okresId);
                        //VAT_Forms.Create(item.Web, klientId, okresId);
                        break;
                    case "Firma":
                        //PDS_Forms.Create(item.Web, klientId, okresId);
                        break;
                    case "Osoba fizyczna":
                        if (mask.StartsWith("ZUS-") || string.IsNullOrEmpty(mask))
                            ZUS_Forms.Manage_ZUS_Form(item.Web, okresId, klientItem, iok);
                        if (mask.StartsWith("PD-") || string.IsNullOrEmpty(mask))
                            PD_Forms.Create_PD_Form(item.Web, okresId, klientItem, iok);
                        //PDS_Forms.Create(item.Web, klientId, okresId);
                        //VAT_Forms.Create(item.Web, klientId, okresId);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
