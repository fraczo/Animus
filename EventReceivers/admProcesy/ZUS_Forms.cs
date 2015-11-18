using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using BLL;
using BLL.Models;

namespace EventReceivers.admProcesy
{
    public class ZUS_Forms
    {
        const string ctZUS = @"Rozliczenie ZUS";

        internal static void Manage_ZUS_Form(SPWeb web, int okresId, SPListItem klientItem, BLL.Models.Klient iok)
        {
            if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "ZUS-*"))
            {
                string key = BLL.tabZadania.Define_KEY(ctZUS, klientItem.ID, okresId);
                if (BLL.tabZadania.Check_KEY_IsAllowed(key, web, 0))
                {
                    string ct = "Rozliczenie ZUS";
                    BLL.tabZadania.Create_ctZUS_Form(web, ct, klientItem.ID, okresId, key, klientItem, iok);
                }
            }

        }
    }
}
