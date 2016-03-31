using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Workflow;

namespace Animus.Features.Workflows
{
    [Guid("72dc5d1d-71ec-4fd7-91f7-ebc7af89ff6d")]
    public class WorkflowsEventReceiver : SPFeatureReceiver
    {

        private string workFlowHistoryListName = "Workflow History";
        private string workFlowTaskListName = "Workflow Tasks";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {

            SPSite site = properties.Feature.Parent as SPSite;
            SPWeb web = site.RootWeb;

            try
            {                
                //swfStratyZLatUbieglych
                BLL.Workflows.AssociateSiteWorkflow(web, "188a4129-23b8-4c76-9462-3dca1a2ee653", "Generator rekordów - Straty z lat ubiegłych", workFlowTaskListName, workFlowHistoryListName);
                
                //swfObslugaKolejkiWiadomosci
                BLL.Workflows.AssociateSiteWorkflow(web, "708534f6-6f8d-4cfc-ab14-49cc67241987", "Obsługa kolejki wiadomości", workFlowTaskListName, workFlowHistoryListName);

                //swfObslugaKartKontrolnych
                BLL.Workflows.AssociateSiteWorkflow(web, "fb837195-86f8-4c88-8156-f8b9a3ba8462", "Obsługa kart kontrolnych", workFlowTaskListName, workFlowHistoryListName);
            
            }
            catch (Exception ex)
            {
                var result = ElasticEmail.EmailGenerator.ReportError(ex, site.Url);
            }
        }
    }
}
