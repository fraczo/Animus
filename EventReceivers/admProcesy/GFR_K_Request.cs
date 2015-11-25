using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace EventReceivers.admProcesy
{
    public class GFR_K_Request
    {
        public static void Create(Microsoft.SharePoint.SPListItem item)
        {
            BLL.Logger.LogEvent_Procedure("GFR_K_Request.Create", item, "start");

            int okresId = BLL.Tools.Get_LookupId(item, "selOkres");
            int klientId = BLL.Tools.Get_LookupId(item, "selKlient");
            string mask = BLL.Tools.Get_Text(item, "colMaskaSerwisu");

            if (okresId > 0 && klientId > 0)
            {
                BLL.Models.Klient iok = new BLL.Models.Klient(item.Web, klientId);

                SPListItem klientItem = BLL.tabKlienci.Get_KlientById(item.Web, klientId);

                BLL.tabKartyKontrolne.Ensure_KartaKontrolna(item.Web, klientId, okresId, iok);

                switch (iok.TypKlienta)
                {
                    case "KPiR":
                        if (mask.StartsWith("ZUS-") || string.IsNullOrEmpty(mask))
                            ZUS_Forms.Manage_ZUS_Form(item.Web, okresId, klientItem, iok);
                        if (mask.StartsWith("PD-") || string.IsNullOrEmpty(mask))
                            PD_Forms.Create_PD_Form(item.Web, okresId, klientItem, iok);
                        if (mask.StartsWith("VAT-") || string.IsNullOrEmpty(mask))
                            VAT_Forms.Create_VAT_Form(item.Web, okresId, klientItem, iok);
                        if (mask.Equals("RBR") || string.IsNullOrEmpty(mask))
                            RBR_Forms.Create_RBR_Form(item.Web, okresId, klientItem, iok);
                        if (mask.Equals("RB") || string.IsNullOrEmpty(mask))
                            RB_Forms.Create_RB_Form(item.Web, okresId, klientItem, iok);
                        break;
                    case "KSH":
                        if (mask.StartsWith("ZUS-") || string.IsNullOrEmpty(mask))
                            ZUS_Forms.Manage_ZUS_Form(item.Web, okresId, klientItem, iok);
                        if (mask.StartsWith("PDS-") || string.IsNullOrEmpty(mask))
                            PDS_Forms.Create_PDS_Form(item.Web, okresId, klientItem, iok);
                        if (mask.StartsWith("VAT-") || string.IsNullOrEmpty(mask))
                            VAT_Forms.Create_VAT_Form(item.Web, okresId, klientItem, iok);
                        if (mask.Equals("RBR") || string.IsNullOrEmpty(mask))
                            RBR_Forms.Create_RBR_Form(item.Web, okresId, klientItem, iok);
                        break;
                    case "Firma":
                        if (mask.StartsWith("PD-") || string.IsNullOrEmpty(mask))
                            PD_Forms.Create_PD_Form(item.Web, okresId, klientItem, iok);
                        if (mask.StartsWith("VAT-") || string.IsNullOrEmpty(mask))
                            VAT_Forms.Create_VAT_Form(item.Web, okresId, klientItem, iok);
                        break;
                    case "Osoba fizyczna":
                        if (mask.StartsWith("ZUS-") || string.IsNullOrEmpty(mask))
                            ZUS_Forms.Manage_ZUS_Form(item.Web, okresId, klientItem, iok);
                        if (mask.StartsWith("PD-") || string.IsNullOrEmpty(mask))
                            PD_Forms.Create_PD_Form(item.Web, okresId, klientItem, iok);
                        break;

                    default:
                        break;
                }
            }

            BLL.Logger.LogEvent_Procedure("GFR_K_Request.Create", item, "end");
        }
    }
}
