using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace Animus.Features.ObslugaWiadomosciTJ
{
    [Guid("f4aed74c-2d23-4a22-9488-8669c0570675")]
    public class ObslugaWiadomosciTJEventReceiver : SPFeatureReceiver
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
