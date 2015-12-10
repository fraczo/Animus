using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace Animus.Features.MsgMgmtTJ
{
    [Guid("5d3c438b-67a9-4b7e-87f6-48c287db9fe3")]
    public class ObslugaWiadomosciEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var site = properties.Feature.Parent as SPSite;
            TimerJobs.ObslugaWiadomosciTJ.CreateTimerJob(site);
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var site = properties.Feature.Parent as SPSite;
            TimerJobs.ObslugaWiadomosciTJ.DelteTimerJob(site);
        }

    }
}
