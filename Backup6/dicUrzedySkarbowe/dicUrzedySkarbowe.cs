using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace EventReceivers.dicUrzedySkarbowe
{
    public class dicUrzedySkarbowe : SPItemEventReceiver
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
                // aktualizuje pole opisowe _konta

                item["_Konta"] = String.Format(@"{0}{1}{2}",
                    item["colPIT_Konto"] != null ? item["colPIT_Konto"] + " - PIT<br>" : string.Empty,
                    item["colCIT_Konto"] != null ? item["colCIT_Konto"] + " - CIT<br>" : string.Empty,
                    item["colVAT_Konto"] != null ? item["colVAT_Konto"] + " - VAT" : string.Empty);
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
