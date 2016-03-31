using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using BLL;

namespace EventReceivers.admProcesy
{
    internal class PDS_Forms
    {
        const string ctPDS = @"Rozliczenie podatku dochodowego spółki";

        internal static void Create_PDS_Form(SPWeb web, int okresId, SPListItem klientItem, BLL.Models.Klient iok)
        {
            if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "PDS-*"))
            {
                string key = BLL.tabZadania.Define_KEY(ctPDS, klientItem.ID, okresId);
                if (BLL.tabZadania.Check_KEY_IsAllowed(key, web, 0))
                {
                    BLL.tabZadania.Create_ctPD_Form(web, ctPDS, klientItem.ID, okresId, key, klientItem, iok);
                }
            }
        }
    }
}
