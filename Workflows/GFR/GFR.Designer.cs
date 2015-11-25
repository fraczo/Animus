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

namespace Workflows.GFR
{
    public sealed partial class GFR
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
            System.Workflow.Activities.CodeCondition codecondition1 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Runtime.CorrelationToken correlationtoken1 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            this.UpdatStatusZadania = new System.Workflow.Activities.CodeActivity();
            this.logToHistoryListActivity4 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.Run_GFR_Create = new System.Workflow.Activities.CodeActivity();
            this.logToHistoryListActivity3 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.CT_GFR = new System.Workflow.Activities.IfElseBranchActivity();
            this.logToHistoryListActivity2 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.ifCT_GFR = new System.Workflow.Activities.IfElseActivity();
            this.logToHistoryListActivity1 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.onWorkflowActivated1 = new Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated();
            // 
            // UpdatStatusZadania
            // 
            this.UpdatStatusZadania.Name = "UpdatStatusZadania";
            this.UpdatStatusZadania.ExecuteCode += new System.EventHandler(this.UpdatStatusZadania_ExecuteCode);
            // 
            // logToHistoryListActivity4
            // 
            this.logToHistoryListActivity4.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity4.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity4.HistoryDescription = "";
            this.logToHistoryListActivity4.HistoryOutcome = "Aktualizacja statusu zlecenia";
            this.logToHistoryListActivity4.Name = "logToHistoryListActivity4";
            this.logToHistoryListActivity4.OtherData = "";
            this.logToHistoryListActivity4.UserId = -1;
            // 
            // Run_GFR_Create
            // 
            this.Run_GFR_Create.Name = "Run_GFR_Create";
            this.Run_GFR_Create.ExecuteCode += new System.EventHandler(this.Select_Klienci_ExecuteCode);
            // 
            // logToHistoryListActivity3
            // 
            this.logToHistoryListActivity3.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity3.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity3.HistoryDescription = "";
            this.logToHistoryListActivity3.HistoryOutcome = "Generowanie zleceń generowania formatek";
            this.logToHistoryListActivity3.Name = "logToHistoryListActivity3";
            this.logToHistoryListActivity3.OtherData = "";
            this.logToHistoryListActivity3.UserId = -1;
            // 
            // CT_GFR
            // 
            this.CT_GFR.Activities.Add(this.logToHistoryListActivity3);
            this.CT_GFR.Activities.Add(this.Run_GFR_Create);
            this.CT_GFR.Activities.Add(this.logToHistoryListActivity4);
            this.CT_GFR.Activities.Add(this.UpdatStatusZadania);
            codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.isCT_GFR);
            this.CT_GFR.Condition = codecondition1;
            this.CT_GFR.Name = "CT_GFR";
            // 
            // logToHistoryListActivity2
            // 
            this.logToHistoryListActivity2.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity2.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity2.HistoryDescription = "";
            this.logToHistoryListActivity2.HistoryOutcome = "STOP";
            this.logToHistoryListActivity2.Name = "logToHistoryListActivity2";
            this.logToHistoryListActivity2.OtherData = "";
            this.logToHistoryListActivity2.UserId = -1;
            // 
            // ifCT_GFR
            // 
            this.ifCT_GFR.Activities.Add(this.CT_GFR);
            this.ifCT_GFR.Name = "ifCT_GFR";
            // 
            // logToHistoryListActivity1
            // 
            this.logToHistoryListActivity1.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity1.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity1.HistoryDescription = "";
            this.logToHistoryListActivity1.HistoryOutcome = "START";
            this.logToHistoryListActivity1.Name = "logToHistoryListActivity1";
            this.logToHistoryListActivity1.OtherData = "";
            this.logToHistoryListActivity1.UserId = -1;
            activitybind2.Name = "GFR";
            activitybind2.Path = "workflowId";
            // 
            // onWorkflowActivated1
            // 
            correlationtoken1.Name = "workflowToken";
            correlationtoken1.OwnerActivityName = "GFR";
            this.onWorkflowActivated1.CorrelationToken = correlationtoken1;
            this.onWorkflowActivated1.EventName = "OnWorkflowActivated";
            this.onWorkflowActivated1.Name = "onWorkflowActivated1";
            activitybind1.Name = "GFR";
            activitybind1.Path = "workflowProperties";
            this.onWorkflowActivated1.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.onWorkflowActivated1_Invoked);
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            // 
            // GFR
            // 
            this.Activities.Add(this.onWorkflowActivated1);
            this.Activities.Add(this.logToHistoryListActivity1);
            this.Activities.Add(this.ifCT_GFR);
            this.Activities.Add(this.logToHistoryListActivity2);
            this.Name = "GFR";
            this.CanModifyActivities = false;

        }

        #endregion

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity4;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity3;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity2;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity1;

        private CodeActivity UpdatStatusZadania;

        private IfElseBranchActivity CT_GFR;

        private IfElseActivity ifCT_GFR;

        private CodeActivity Run_GFR_Create;

        private Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated onWorkflowActivated1;












    }
}
