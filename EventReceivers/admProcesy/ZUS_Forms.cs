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

        /// <summary>
        /// jeżeli klient ma przypisane serwisy ZUS zgodne z zadaną maską i klucz KEY nie jest zdublowany
        /// uruchamiana jest procedura tworzenia zadania
        /// </summary>
        internal static void Manage_ZUS_Form(SPWeb web, int okresId, SPListItem klientItem, BLL.Models.Klient iok)
        {
            if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "ZUS-*"))
            {
                string key = BLL.tabZadania.Define_KEY(ctZUS, klientItem.ID, okresId);
                if (BLL.tabZadania.Check_KEY_IsAllowed(key, web, 0))
                {
                    BLL.tabZadania.Create_ctZUS_Form(web, ctZUS, klientItem.ID, okresId, key, klientItem, iok);
                }
            }

        }
    }
}
