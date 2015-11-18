using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using BLL;
using BLL.Models;

namespace EventReceivers.admProcesy
{
    public class VAT_Forms
    {
        const string ctVAT = "Rozliczenie podatku VAT";

        internal static void Create_VAT_Form(SPWeb web, int okresId, SPListItem klientItem, Klient iok)
        {
            if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "VAT-*"))
            {
                string key = BLL.tabZadania.Define_KEY(ctVAT, klientItem.ID, okresId);
                if (BLL.tabZadania.Check_KEY_IsAllowed(key, web, 0))
                {
                    BLL.tabZadania.Create_ctVAT_Form(web, ctVAT, klientItem.ID, okresId, key, klientItem, iok);
                }
            }
        }
    }
}
