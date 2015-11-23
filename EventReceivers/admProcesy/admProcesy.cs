using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using BLL;

namespace EventReceivers.admProcesy
{
    public class admProcesy : SPItemEventReceiver
    {
       public override void ItemAdded(SPItemEventProperties properties)
       {
           //this.EventFiringEnabled = false;

           SPListItem item = properties.ListItem;

           BLL.Logger.LogEvent("admProcsy.ItemAdded",item.ID.ToString());

           item["enumStatusZlecenia"] = "Obsługa";
           this.EventFiringEnabled = false;
           item.SystemUpdate();
           this.EventFiringEnabled = true;

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
                   case "Obsługa wiadomości":
                       ObslugaWiadomosci.Execute(item);
                       break;
                   default:
                       break;
               }

               BLL.Logger.LogEvent("admProcsy.ItemAdded.end", item.ID.ToString());
               //this.EventFiringEnabled = true;  
               item.Delete();
           }
           catch (Exception ex)
           {
               BLL.Logger.LogEvent("admProcsy.ItemAdded.error", ex.ToString());

               BLL.Tools.Set_Text(item, "_Memo", ex.ToString());
               BLL.Tools.Set_Text(item, "enumStatusZlecenia", "Anulowane");
               item.SystemUpdate();
           }
       }
    }
}
