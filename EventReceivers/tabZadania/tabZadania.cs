using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace EventReceivers.tabZadania
{
    public class tabZadania : SPItemEventReceiver
    {
        public override void ItemAdded(SPItemEventProperties properties)
        {
            Execute(properties);
        }

        public override void ItemUpdated(SPItemEventProperties properties)
        {
            Execute(properties);
        }

        private void Execute(SPItemEventProperties properties)
        {
            this.EventFiringEnabled = false;
            SPListItem item = properties.ListItem;
            BLL.Logger.LogEvent_EventReceiverInitiated(item);

            try
            {
                switch (item.ContentType.Name)
                {
                    case "Wiadomość z ręki":
                    case "Wiadomość z szablonu":
                    case "Wiadomość grupowa":
                    case "Wiadomość grupowa z szablonu":
                        BLL.tabWiadomosci.CreateMailMessage(item);
                        break;
                }

                item.SystemUpdate();
            }
            catch (Exception ex)
            {
                BLL.Logger.LogEvent(properties.WebUrl, ex.ToString());
                var result = ElasticEmail.EmailGenerator.ReportError(ex, properties.WebUrl.ToString());
            }
            finally
            {
                BLL.Logger.LogEvent_EventReceiverCompleted(item);
                this.EventFiringEnabled = true;
            }
        }

    }
}
