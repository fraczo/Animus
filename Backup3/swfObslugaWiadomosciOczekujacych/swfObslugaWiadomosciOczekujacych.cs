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

namespace Workflows.swfObslugaWiadomosciOczekujacych
{
    public sealed partial class swfObslugaWiadomosciOczekujacych : SequentialWorkflowActivity
    {
        public swfObslugaWiadomosciOczekujacych()
        {
            InitializeComponent();
        }

        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();
        public String logSelected_HistoryDescription = default(System.String);
        public String logCurrentMessage_HistoryDescription = default(System.String);
        private Array results;
        private IEnumerator myEnum;

        private void Select_ListaWiadomosciOczekujacych_ExecuteCode(object sender, EventArgs e)
        {
            results = BLL.tabWiadomosci.Select_Batch(workflowProperties.Web);
            myEnum = results.GetEnumerator();

            if (results != null) logSelected_HistoryOutcome = results.Length.ToString();
            else logSelected_HistoryOutcome = "0";
        }

        private void whileRecordExist(object sender, ConditionalEventArgs e)
        {
            if (myEnum.MoveNext() && myEnum != null) e.Result = true;
            else e.Result = false;
        }

        private void Initialize_ChildWorkflow_ExecuteCode(object sender, EventArgs e)
        {
            SPListItem item = myEnum.Current as SPListItem;

            BLL.Workflows.StartWorkflow(item, "Obsługa wiadomości");

            logSelected_HistoryOutcome = item.ID.ToString();

        }

        private void onWorkflowActivated1_Invoked(object sender, ExternalDataEventArgs e)
        {
            Debug.WriteLine("swfObslugaWiadomosciOczekujacych - ACTIVATED");
        }

        public String logSelected_HistoryOutcome = default(System.String);
        public String logCurrentMessage_HistoryOutcome = default(System.String);
        public String logWorkflow_HistoryOutcome = default(System.String);
        public String logErrorMessage_HistoryDescription = default(System.String);
        public String logErrorMessage_HistoryOutcome = default(System.String);

        private void cmdErrorHandler_ExecuteCode(object sender, EventArgs e)
        {
            FaultHandlerActivity fa = ((Activity)sender).Parent as FaultHandlerActivity;
            if (fa != null)
            {
                Debug.WriteLine(fa.Fault.Source);
                Debug.WriteLine(fa.Fault.Message);
                Debug.WriteLine(fa.Fault.StackTrace);

                logErrorMessage_HistoryDescription = string.Format("{0}::{1}",
                    fa.Fault.Message,
                    fa.Fault.StackTrace);

                ElasticEmail.EmailGenerator.ReportErrorFromWorkflow(workflowProperties, fa.Fault.Message, fa.Fault.StackTrace);
            }

        }


    }
}
