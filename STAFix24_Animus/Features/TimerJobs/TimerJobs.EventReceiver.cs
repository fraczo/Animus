using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace Animus.Features.TimerJobs
{
    [Guid("115e24b1-402c-40e1-8891-b9797c9eb321")]
    public class TimerJob_ObslugaWiadomosciEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite site = properties.Feature.Parent as SPSite;

            try
            {
                Animus.TimerJobs.ObslugaWiadomosciTJ.CreateTimerJob(site);
            }
            catch (Exception ex)
            {
                ElasticEmail.EmailGenerator.ReportError(ex, site.Url);
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var site = properties.Feature.Parent as SPSite;
            Animus.TimerJobs.ObslugaWiadomosciTJ.DelteTimerJob(site);
        }
    }
}
