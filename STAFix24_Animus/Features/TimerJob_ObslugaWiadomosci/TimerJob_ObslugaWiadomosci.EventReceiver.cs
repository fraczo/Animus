using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace Animus.Features.TimerJob_ObslugaWiadomosci
{
    [Guid("115e24b1-402c-40e1-8891-b9797c9eb321")]
    public class TimerJob_ObslugaWiadomosciEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var site = properties.Feature.Parent as SPSite;
            TimerJobs.ObslugaWiadomosci.CreateTimerJob(site);
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var site = properties.Feature.Parent as SPSite;
            TimerJobs.ObslugaWiadomosci.DelteTimerJob(site);
        }
    }
}
