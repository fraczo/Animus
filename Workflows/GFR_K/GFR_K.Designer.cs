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

namespace Workflows.GFR_K
{
    public sealed partial class GFR_K
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
            this.logProcCompleted = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.Run_GFR_K_Create = new System.Workflow.Activities.CodeActivity();
            this.logCase_CT_GFRK = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.ifCT_GFRK = new System.Workflow.Activities.IfElseBranchActivity();
            this.faultHandlersActivity1 = new System.Workflow.ComponentModel.FaultHandlersActivity();
            this.cancellationHandlerActivity1 = new System.Workflow.ComponentModel.CancellationHandlerActivity();
            this.logStatus_Zakończony = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.Delete_Item = new System.Workflow.Activities.CodeActivity();
            this.Status_Zakonczony = new System.Workflow.Activities.CodeActivity();
            this.Test_CT = new System.Workflow.Activities.IfElseActivity();
            this.logStatus_Obsługa = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.Status_Obsluga = new System.Workflow.Activities.CodeActivity();
            this.onWorkflowActivated1 = new Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated();
            // 
            // logProcCompleted
            // 
            this.logProcCompleted.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logProcCompleted.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logProcCompleted.HistoryDescription = "";
            this.logProcCompleted.HistoryOutcome = "Case.Completed";
            this.logProcCompleted.Name = "logProcCompleted";
            this.logProcCompleted.OtherData = "";
            this.logProcCompleted.UserId = -1;
            // 
            // Run_GFR_K_Create
            // 
            this.Run_GFR_K_Create.Name = "Run_GFR_K_Create";
            this.Run_GFR_K_Create.ExecuteCode += new System.EventHandler(this.Run_GFR_K_Create_ExecuteCode);
            // 
            // logCase_CT_GFRK
            // 
            this.logCase_CT_GFRK.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logCase_CT_GFRK.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logCase_CT_GFRK.HistoryDescription = "";
            this.logCase_CT_GFRK.HistoryOutcome = "Case=CT_GFRK";
            this.logCase_CT_GFRK.Name = "logCase_CT_GFRK";
            this.logCase_CT_GFRK.OtherData = "";
            this.logCase_CT_GFRK.UserId = -1;
            // 
            // ifCT_GFRK
            // 
            this.ifCT_GFRK.Activities.Add(this.logCase_CT_GFRK);
            this.ifCT_GFRK.Activities.Add(this.Run_GFR_K_Create);
            this.ifCT_GFRK.Activities.Add(this.logProcCompleted);
            codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.isCT_GFRK);
            this.ifCT_GFRK.Condition = codecondition1;
            this.ifCT_GFRK.Name = "ifCT_GFRK";
            // 
            // faultHandlersActivity1
            // 
            this.faultHandlersActivity1.Name = "faultHandlersActivity1";
            // 
            // cancellationHandlerActivity1
            // 
            this.cancellationHandlerActivity1.Name = "cancellationHandlerActivity1";
            // 
            // logStatus_Zakończony
            // 
            this.logStatus_Zakończony.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logStatus_Zakończony.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logStatus_Zakończony.HistoryDescription = "";
            this.logStatus_Zakończony.HistoryOutcome = "Status=Zakończony";
            this.logStatus_Zakończony.Name = "logStatus_Zakończony";
            this.logStatus_Zakończony.OtherData = "";
            this.logStatus_Zakończony.UserId = -1;
            // 
            // Delete_Item
            // 
            this.Delete_Item.Name = "Delete_Item";
            this.Delete_Item.ExecuteCode += new System.EventHandler(this.Delete_Item_ExecuteCode);
            // 
            // Status_Zakonczony
            // 
            this.Status_Zakonczony.Name = "Status_Zakonczony";
            this.Status_Zakonczony.ExecuteCode += new System.EventHandler(this.Status_Zakonczony_ExecuteCode);
            // 
            // Test_CT
            // 
            this.Test_CT.Activities.Add(this.ifCT_GFRK);
            this.Test_CT.Name = "Test_CT";
            // 
            // logStatus_Obsługa
            // 
            this.logStatus_Obsługa.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logStatus_Obsługa.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logStatus_Obsługa.HistoryDescription = "";
            this.logStatus_Obsługa.HistoryOutcome = "Status=Obsługa";
            this.logStatus_Obsługa.Name = "logStatus_Obsługa";
            this.logStatus_Obsługa.OtherData = "";
            this.logStatus_Obsługa.UserId = -1;
            // 
            // Status_Obsluga
            // 
            this.Status_Obsluga.Name = "Status_Obsluga";
            this.Status_Obsluga.ExecuteCode += new System.EventHandler(this.Status_Obsluga_ExecuteCode);
            activitybind2.Name = "GFR_K";
            activitybind2.Path = "workflowId";
            // 
            // onWorkflowActivated1
            // 
            correlationtoken1.Name = "workflowToken";
            correlationtoken1.OwnerActivityName = "GFR_K";
            this.onWorkflowActivated1.CorrelationToken = correlationtoken1;
            this.onWorkflowActivated1.EventName = "OnWorkflowActivated";
            this.onWorkflowActivated1.Name = "onWorkflowActivated1";
            activitybind1.Name = "GFR_K";
            activitybind1.Path = "workflowProperties";
            this.onWorkflowActivated1.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.onWorkflowActivated1_Invoked);
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            // 
            // GFR_K
            // 
            this.Activities.Add(this.onWorkflowActivated1);
            this.Activities.Add(this.Status_Obsluga);
            this.Activities.Add(this.logStatus_Obsługa);
            this.Activities.Add(this.Test_CT);
            this.Activities.Add(this.Status_Zakonczony);
            this.Activities.Add(this.Delete_Item);
            this.Activities.Add(this.logStatus_Zakończony);
            this.Activities.Add(this.cancellationHandlerActivity1);
            this.Activities.Add(this.faultHandlersActivity1);
            this.Name = "GFR_K";
            this.CanModifyActivities = false;

        }

        #endregion

        private CodeActivity Delete_Item;

        private CodeActivity Status_Obsluga;

        private FaultHandlersActivity faultHandlersActivity1;

        private CancellationHandlerActivity cancellationHandlerActivity1;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logProcCompleted;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logCase_CT_GFRK;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logStatus_Zakończony;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logStatus_Obsługa;

        private CodeActivity Status_Zakonczony;

        private IfElseBranchActivity ifCT_GFRK;

        private IfElseActivity Test_CT;

        private CodeActivity Run_GFR_K_Create;

        private Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated onWorkflowActivated1;

















    }
}
