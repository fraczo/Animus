using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL;
using Microsoft.SharePoint;

namespace EventReceivers.admProcesy
{
    public class RBR_Forms
    {
        const string ctRBR = @"Rozliczenie z biurem rachunkowym";

        internal static void Create_RBR_Form(SPWeb web, int okresId, SPListItem klientItem, BLL.Models.Klient iok)
        {
            if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "RBR"))
            {
                string key = BLL.tabZadania.Define_KEY(ctRBR, klientItem.ID, okresId);
                if (BLL.tabZadania.Check_KEY_IsAllowed(key, web, 0))
                {
                    BLL.tabZadania.Create_ctRBR_Form(web, ctRBR, klientItem.ID, okresId, key, klientItem, iok);
                }
            }
        }
    }
}
