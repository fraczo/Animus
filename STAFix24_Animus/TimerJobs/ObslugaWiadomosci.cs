using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Animus.TimerJobs
{
    public class ObslugaWiadomosci : Microsoft.SharePoint.Administration.SPJobDefinition
    {
        public static void CreateTimerJob(SPSite site)
        {
            var timerJob = new ObslugaWiadomosci(site);
            timerJob.Schedule = new SPMinuteSchedule
            {
                BeginSecond = 0,
                EndSecond = 0,
                Interval = 1
            };

            timerJob.Update();
        }

        public static void DelteTimerJob(SPSite site)
        {
            site.WebApplication.JobDefinitions
                .OfType<ObslugaWiadomosci>()
                .Where(i => string.Equals(i.SiteUrl, site.Url, StringComparison.InvariantCultureIgnoreCase))
                .ToList()
                .ForEach(i => i.Delete());
        }

        public ObslugaWiadomosci()
            : base()
        {

        }

        public ObslugaWiadomosci(SPSite site)
            : base(string.Format("XXXXX001 ({0})", site.Url), site.WebApplication, null, SPJobLockType.Job)
        {
            //Animus_Obsluga wiadomosci Timer Job ({0})
            Title = Name;
            SiteUrl = site.Url;
        }

        public string SiteUrl
        {
            get { return (string)this.Properties["SiteUrl"]; }
            set { this.Properties["SiteUrl"] = value; }
        }

        public override void Execute(Guid targetInstanceId)
        {
            using (var site = new SPSite(SiteUrl))
            {
                //SPList list = site.RootWeb.Lists.TryGetList("admProcesy");

                //SPListItem item = list.AddItem();
                //item["ContentType"] = "Obsługa wiadomości";
                //item.SystemUpdate();


                BLL.Workflows.StartSiteWorkflow(site, "Obsługa wiadomości oczekujących");

            }
        }
    }
}
