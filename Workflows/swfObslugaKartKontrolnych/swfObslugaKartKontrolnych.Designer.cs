using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace Workflows.swfObslugaKartKontrolnych
{
    public sealed partial class swfObslugaKartKontrolnych
    {
        #region Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Activities.CodeCondition codecondition1 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.ComponentModel.ActivityBind activitybind3 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind4 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind6 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Runtime.CorrelationToken correlationtoken1 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind5 = new System.Workflow.ComponentModel.ActivityBind();
            this.logErrorMessage = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.cmdErrorHandler = new System.Workflow.Activities.CodeActivity();
            this.logWorkflowStarted = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.cmdRun_ObslugaKartyKontrolnejWF = new System.Workflow.Activities.CodeActivity();
            this.faultHandlerActivity1 = new System.Workflow.ComponentModel.FaultHandlerActivity();
            this.sequenceActivity1 = new System.Workflow.Activities.SequenceActivity();
            this.faultHandlersActivity1 = new System.Workflow.ComponentModel.FaultHandlersActivity();
            this.logEnd = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.whileKartaKontrolna = new System.Workflow.Activities.WhileActivity();
            this.logSelected = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.cmdGet_KartyKontrolne = new System.Workflow.Activities.CodeActivity();
            this.onWorkflowActivated1 = new Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated();
            // 
            // logErrorMessage
            // 
            this.logErrorMessage.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logErrorMessage.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            activitybind1.Name = "swfObslugaKartKontrolnych";
            activitybind1.Path = "logErrorMessage_HistoryDescription";
            this.logErrorMessage.HistoryOutcome = "";
            this.logErrorMessage.Name = "logErrorMessage";
            this.logErrorMessage.OtherData = "";
            this.logErrorMessage.UserId = -1;
            this.logErrorMessage.SetBinding(Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity.HistoryDescriptionProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            // 
            // cmdErrorHandler
            // 
            this.cmdErrorHandler.Name = "cmdErrorHandler";
            this.cmdErrorHandler.ExecuteCode += new System.EventHandler(this.cmdErrorHandler_ExecuteCode);
            // 
            // logWorkflowStarted
            // 
            this.logWorkflowStarted.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logWorkflowStarted.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logWorkflowStarted.HistoryDescription = "WF uruchomiony";
            activitybind2.Name = "swfObslugaKartKontrolnych";
            activitybind2.Path = "logWorkflowStarted_HistoryOutcome";
            this.logWorkflowStarted.Name = "logWorkflowStarted";
            this.logWorkflowStarted.OtherData = "";
            this.logWorkflowStarted.UserId = -1;
            this.logWorkflowStarted.SetBinding(Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity.HistoryOutcomeProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            // 
            // cmdRun_ObslugaKartyKontrolnejWF
            // 
            this.cmdRun_ObslugaKartyKontrolnejWF.Name = "cmdRun_ObslugaKartyKontrolnejWF";
            this.cmdRun_ObslugaKartyKontrolnejWF.ExecuteCode += new System.EventHandler(this.cmdRun_ObslugaKartyKontrolnejWF_ExecuteCode);
            // 
            // faultHandlerActivity1
            // 
            this.faultHandlerActivity1.Activities.Add(this.cmdErrorHandler);
            this.faultHandlerActivity1.Activities.Add(this.logErrorMessage);
            this.faultHandlerActivity1.FaultType = typeof(System.SystemException);
            this.faultHandlerActivity1.Name = "faultHandlerActivity1";
            // 
            // sequenceActivity1
            // 
            this.sequenceActivity1.Activities.Add(this.cmdRun_ObslugaKartyKontrolnejWF);
            this.sequenceActivity1.Activities.Add(this.logWorkflowStarted);
            this.sequenceActivity1.Name = "sequenceActivity1";
            // 
            // faultHandlersActivity1
            // 
            this.faultHandlersActivity1.Activities.Add(this.faultHandlerActivity1);
            this.faultHandlersActivity1.Name = "faultHandlersActivity1";
            // 
            // logEnd
            // 
            this.logEnd.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logEnd.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logEnd.HistoryDescription = "Zakończony";
            this.logEnd.HistoryOutcome = "";
            this.logEnd.Name = "logEnd";
            this.logEnd.OtherData = "";
            this.logEnd.UserId = -1;
            // 
            // whileKartaKontrolna
            // 
            this.whileKartaKontrolna.Activities.Add(this.sequenceActivity1);
            codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.whileKKExist);
            this.whileKartaKontrolna.Condition = codecondition1;
            this.whileKartaKontrolna.Name = "whileKartaKontrolna";
            // 
            // logSelected
            // 
            this.logSelected.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logSelected.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            activitybind3.Name = "swfObslugaKartKontrolnych";
            activitybind3.Path = "logSelected_HistoryDescription";
            activitybind4.Name = "swfObslugaKartKontrolnych";
            activitybind4.Path = "logSelected_HistoryOutcome";
            this.logSelected.Name = "logSelected";
            this.logSelected.OtherData = "";
            this.logSelected.UserId = -1;
            this.logSelected.SetBinding(Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity.HistoryDescriptionProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind3)));
            this.logSelected.SetBinding(Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity.HistoryOutcomeProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind4)));
            // 
            // cmdGet_KartyKontrolne
            // 
            this.cmdGet_KartyKontrolne.Name = "cmdGet_KartyKontrolne";
            this.cmdGet_KartyKontrolne.ExecuteCode += new System.EventHandler(this.cmdGet_KartyKontrolne_ExecuteCode);
            activitybind6.Name = "swfObslugaKartKontrolnych";
            activitybind6.Path = "workflowId";
            // 
            // onWorkflowActivated1
            // 
            correlationtoken1.Name = "workflowToken";
            correlationtoken1.OwnerActivityName = "swfObslugaKartKontrolnych";
            this.onWorkflowActivated1.CorrelationToken = correlationtoken1;
            this.onWorkflowActivated1.EventName = "OnWorkflowActivated";
            this.onWorkflowActivated1.Name = "onWorkflowActivated1";
            activitybind5.Name = "swfObslugaKartKontrolnych";
            activitybind5.Path = "workflowProperties";
            this.onWorkflowActivated1.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.onWorkflowActivated1_Invoked);
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind6)));
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind5)));
            // 
            // swfObslugaKartKontrolnych
            // 
            this.Activities.Add(this.onWorkflowActivated1);
            this.Activities.Add(this.cmdGet_KartyKontrolne);
            this.Activities.Add(this.logSelected);
            this.Activities.Add(this.whileKartaKontrolna);
            this.Activities.Add(this.logEnd);
            this.Activities.Add(this.faultHandlersActivity1);
            this.Name = "swfObslugaKartKontrolnych";
            this.CanModifyActivities = false;

        }

        #endregion

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logErrorMessage;

        private CodeActivity cmdErrorHandler;

        private FaultHandlerActivity faultHandlerActivity1;

        private FaultHandlersActivity faultHandlersActivity1;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logEnd;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logWorkflowStarted;

        private SequenceActivity sequenceActivity1;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logSelected;

        private CodeActivity cmdRun_ObslugaKartyKontrolnejWF;

        private WhileActivity whileKartaKontrolna;

        private CodeActivity cmdGet_KartyKontrolne;

        private Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated onWorkflowActivated1;



















    }
}
