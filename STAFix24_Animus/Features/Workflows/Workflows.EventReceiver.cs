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
                BLL.Workflows.AssociateSiteWorkflow(web, "b398c228-9469-4f23-986f-3468821729d3", "Wyślij zestawienie godzin", workFlowTaskListName, workFlowHistoryListName);
            }
            catch (Exception ex)
            {
                var result = ElasticEmail.EmailGenerator.ReportError(ex, site.Url);
            }
        }
    }
}
