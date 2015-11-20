using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace Animus.Features.ObslugaWiadomosciTJ
{
    [Guid("5E856AF0-C1D2-4F44-AE5E-289AD42EDF83")]
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
