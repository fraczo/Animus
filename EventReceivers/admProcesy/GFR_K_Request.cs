﻿using System;
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
            if (okresId > 0 && klientId > 0)
            {
                BLL.Models.Klient iok = new BLL.Models.Klient(item.Web, klientId);

                SPListItem klientItem = BLL.tabKlienci.Get_KlientById(item.Web, klientId);

                switch (iok.TypKlienta)
                {
                    case "KPiR":
                        ZUS_Forms.Manage_ZUS_Form(item.Web, okresId, klientItem, iok);
                        //PD_Forms.Create(item.Web, klientId, okresId);
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
                        ZUS_Forms.Manage_ZUS_Form(item.Web, okresId, klientItem, iok);
                        //PD_Forms.Create(item.Web, klientId, okresId);
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