using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace EventReceivers.admProcesy
{
    public class admProcesy : SPItemEventReceiver
    {
       public override void ItemAdded(SPItemEventProperties properties)
       {
           SPListItem item = properties.ListItem;
           item["enumStatusZlecenia"] = "Obsługa";
           item.SystemUpdate();

           try
           {
               switch (item.ContentType.Name)
               {
                   case "Generowanie formatek rozliczeniowych dla klienta":
                       GFR_K_Request.Create(item);
                       break;
                   case "Generowanie formatek rozliczeniowych":
                       GFR_Request.Create(item);
                       break;
                   default:
                       break;
               }

               item.Delete();
               //item.SystemUpdate();
           }
           catch (Exception ex)
           {
               item["enumStatusZlecenia"] = "Anulowany";
               item["_Memo"] = ex.ToString();
               item.SystemUpdate();
           }

       }
    }
}
