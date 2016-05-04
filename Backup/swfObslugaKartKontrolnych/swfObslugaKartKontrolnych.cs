using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using System.Diagnostics;

namespace Workflows.swfObslugaKartKontrolnych
{
    public sealed partial class swfObslugaKartKontrolnych : SequentialWorkflowActivity
    {
        public swfObslugaKartKontrolnych()
        {
            InitializeComponent();
        }

        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();
        Array results;
        private IEnumerator myEnum;

        public String logSelected_HistoryOutcome = default(System.String);
        public String logSelected_HistoryDescription = default(System.String);
        public String logErrorMessage_HistoryDescription = default(System.String);

        private void cmdGet_KartyKontrolne_ExecuteCode(object sender, EventArgs e)
        {
            logSelected_HistoryDescription = "Karty kontrolne do obsługi";

            results = BLL.tabKartyKontrolne.Get_ZwolnioneDoWysylki(workflowProperties.Web);
            myEnum = results.GetEnumerator();

            if (results != null) logSelected_HistoryOutcome = results.Length.ToString();
            else logSelected_HistoryOutcome = "0";

        }

        private void whileKKExist(object sender, ConditionalEventArgs e)
        {
            if (myEnum.MoveNext() && myEnum != null) e.Result = true;
            else e.Result = false;
        }

        private void cmdRun_ObslugaKartyKontrolnejWF_ExecuteCode(object sender, EventArgs e)
        {
            SPListItem item = myEnum.Current as SPListItem;

            BLL.Workflows.StartWorkflow(item, "Przygotuj wiadomość dla klienta");

            logWorkflowStarted_HistoryOutcome = BLL.Tools.Get_LookupValue(item, "selKlient");
        }

        public String logWorkflowStarted_HistoryOutcome = default(System.String);

        private void cmdErrorHandler_ExecuteCode(object sender, EventArgs e)
        {
            FaultHandlerActivity fa = ((Activity)sender).Parent as FaultHandlerActivity;
            if (fa != null)
            {
                logErrorMessage_HistoryDescription = string.Format("{0}::{1}",
                    fa.Fault.Message,
                    fa.Fault.StackTrace);

                ElasticEmail.EmailGenerator.ReportErrorFromWorkflow(workflowProperties, fa.Fault.Message, fa.Fault.StackTrace);
            }
        }

        private void onWorkflowActivated1_Invoked(object sender, ExternalDataEventArgs e)
        {
            Debug.WriteLine("swfObslugaKartKontrolnych - ACTIVATED");
        }
    }
}
