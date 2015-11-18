using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using BLL;

namespace EventReceivers.admProcesy
{
    internal class PD_Forms
    {
        const string ctPD = @"Rozliczenie podatku dochodowego";

        internal static void Create_PD_Form(SPWeb web, int okresId, SPListItem klientItem, BLL.Models.Klient iok)
        {
            if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "PD-*"))
            {
                string key = BLL.tabZadania.Define_KEY(ctPD, klientItem.ID, okresId);
                if (BLL.tabZadania.Check_KEY_IsAllowed(key, web, 0))
                {
                    BLL.tabZadania.Create_ctPD_Form(web, ctPD, klientItem.ID, okresId, key, klientItem, iok);
                }
            }
        }
    }
}
