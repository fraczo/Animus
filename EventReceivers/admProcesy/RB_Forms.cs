using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace EventReceivers.admProcesy
{
    class RB_Forms
    {
        internal static void Create_RB_Form(Microsoft.SharePoint.SPWeb web, int okresId, Microsoft.SharePoint.SPListItem klientItem, BLL.Models.Klient iok)
        {
            if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "RB")) //ograniczony dostęp do tablicy rozliczeń gotówkowych
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite eSite = new SPSite(web.Site.ID))
                    {
                        using (SPWeb eWeb = eSite.OpenWeb())
                        {
                            string key = BLL.tabRozliczeniaGotowkowe.Define_KEY(klientItem.ID, okresId);
                            if (BLL.tabRozliczeniaGotowkowe.Check_KEY_IsAllowed(key, web, 0))
                            {

                                BLL.tabRozliczeniaGotowkowe.Create_ctRB_Form(web, klientItem.ID, okresId, key, klientItem, iok);

                            }
                        }
                    }
                });
            }
        }
    }
}
