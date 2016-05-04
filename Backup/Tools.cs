using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using System.Workflow.ComponentModel;

namespace Workflows
{
    public class Tools
    {
        public static void WriteToHistoryLog(SPWorkflowActivationProperties workflowProperties, string description, string outcome)
        {
            SPWeb web = workflowProperties.Web;
            Guid workflow = workflowProperties.WorkflowId;

            TimeSpan ts = new TimeSpan();
            SPWorkflow.CreateHistoryEvent(web, workflow, 0, web.CurrentUser, ts,
                outcome, description, string.Empty);
        }

        internal static void ReportErrorToHistoryLog(SPWorkflowActivationProperties workflowProperties, object sender, bool sendMail)
        {
            FaultHandlerActivity fa = ((Activity)sender).Parent as FaultHandlerActivity;
            if (fa != null)
            {
                Workflows.Tools.WriteToHistoryLog(
                    workflowProperties,
                    string.Format("{1} ({0})", ((Activity)sender).Name, fa.Fault.Message),
                    fa.Fault.StackTrace);

                if (sendMail)
                {
                    var r = ElasticEmail.EmailGenerator.ReportErrorFromWorkflow(workflowProperties, fa.Fault.Message, fa.Fault.StackTrace);
                }
            }


        }
    }
}
