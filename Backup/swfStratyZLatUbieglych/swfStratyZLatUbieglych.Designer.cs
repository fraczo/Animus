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

namespace Workflows.swfStratyZLatUbieglych
{
    public sealed partial class swfStratyZLatUbieglych
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
            System.Workflow.Runtime.CorrelationToken correlationtoken1 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Activities.CodeCondition codecondition2 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind4 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind3 = new System.Workflow.ComponentModel.ActivityBind();
            this.Append_Records = new System.Workflow.Activities.CodeActivity();
            this.logToHistoryListActivity2 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.Else = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifRecordExist = new System.Workflow.Activities.IfElseBranchActivity();
            this.SprawdzCzyIstnieje = new System.Workflow.Activities.IfElseActivity();
            this.Manage_Records = new System.Workflow.Activities.SequenceActivity();
            this.logEnd = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.sendCompleted = new Microsoft.SharePoint.WorkflowActions.SendEmail();
            this.whileCompleted = new System.Workflow.Activities.WhileActivity();
            this.Get_ExistingKEYs = new System.Workflow.Activities.CodeActivity();
            this.Create_TargetList = new System.Workflow.Activities.CodeActivity();
            this.sendInitiated = new Microsoft.SharePoint.WorkflowActions.SendEmail();
            this.onWorkflowActivated1 = new Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated();
            // 
            // Append_Records
            // 
            this.Append_Records.Name = "Append_Records";
            this.Append_Records.ExecuteCode += new System.EventHandler(this.Append_Records_ExecuteCode);
            // 
            // logToHistoryListActivity2
            // 
            this.logToHistoryListActivity2.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity2.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity2.HistoryDescription = "rekord istnieje";
            this.logToHistoryListActivity2.HistoryOutcome = "";
            this.logToHistoryListActivity2.Name = "logToHistoryListActivity2";
            this.logToHistoryListActivity2.OtherData = "";
            this.logToHistoryListActivity2.UserId = -1;
            // 
            // Else
            // 
            this.Else.Activities.Add(this.Append_Records);
            this.Else.Name = "Else";
            // 
            // ifRecordExist
            // 
            this.ifRecordExist.Activities.Add(this.logToHistoryListActivity2);
            codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.isRecordExist2);
            this.ifRecordExist.Condition = codecondition1;
            this.ifRecordExist.Name = "ifRecordExist";
            // 
            // SprawdzCzyIstnieje
            // 
            this.SprawdzCzyIstnieje.Activities.Add(this.ifRecordExist);
            this.SprawdzCzyIstnieje.Activities.Add(this.Else);
            this.SprawdzCzyIstnieje.Name = "SprawdzCzyIstnieje";
            // 
            // Manage_Records
            // 
            this.Manage_Records.Activities.Add(this.SprawdzCzyIstnieje);
            this.Manage_Records.Name = "Manage_Records";
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
            // sendCompleted
            // 
            this.sendCompleted.BCC = null;
            this.sendCompleted.Body = null;
            this.sendCompleted.CC = null;
            correlationtoken1.Name = "workflowToken";
            correlationtoken1.OwnerActivityName = "swfStratyZLatUbieglych";
            this.sendCompleted.CorrelationToken = correlationtoken1;
            this.sendCompleted.Enabled = false;
            this.sendCompleted.From = null;
            this.sendCompleted.Headers = null;
            this.sendCompleted.IncludeStatus = false;
            this.sendCompleted.Name = "sendCompleted";
            this.sendCompleted.Subject = "swf Straty z lat ubiegłych - completed";
            activitybind1.Name = "swfStratyZLatUbieglych";
            activitybind1.Path = "workflowProperties.OriginatorEmail";
            this.sendCompleted.SetBinding(Microsoft.SharePoint.WorkflowActions.SendEmail.ToProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            // 
            // whileCompleted
            // 
            this.whileCompleted.Activities.Add(this.Manage_Records);
            codecondition2.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.isCompleted);
            this.whileCompleted.Condition = codecondition2;
            this.whileCompleted.Name = "whileCompleted";
            // 
            // Get_ExistingKEYs
            // 
            this.Get_ExistingKEYs.Name = "Get_ExistingKEYs";
            this.Get_ExistingKEYs.ExecuteCode += new System.EventHandler(this.Get_ExistingKEYs_ExecuteCode);
            // 
            // Create_TargetList
            // 
            this.Create_TargetList.Name = "Create_TargetList";
            this.Create_TargetList.ExecuteCode += new System.EventHandler(this.Create_TargetList_ExecuteCode);
            // 
            // sendInitiated
            // 
            this.sendInitiated.BCC = null;
            this.sendInitiated.Body = null;
            this.sendInitiated.CC = null;
            this.sendInitiated.CorrelationToken = correlationtoken1;
            this.sendInitiated.From = null;
            this.sendInitiated.Headers = null;
            this.sendInitiated.IncludeStatus = false;
            this.sendInitiated.Name = "sendInitiated";
            this.sendInitiated.Subject = "swf Straty z lat ubiegłych - started";
            activitybind2.Name = "swfStratyZLatUbieglych";
            activitybind2.Path = "workflowProperties.OriginatorEmail";
            this.sendInitiated.MethodInvoking += new System.EventHandler(this.sendInitiated_MethodInvoking);
            this.sendInitiated.SetBinding(Microsoft.SharePoint.WorkflowActions.SendEmail.ToProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            activitybind4.Name = "swfStratyZLatUbieglych";
            activitybind4.Path = "workflowId";
            // 
            // onWorkflowActivated1
            // 
            this.onWorkflowActivated1.CorrelationToken = correlationtoken1;
            this.onWorkflowActivated1.EventName = "OnWorkflowActivated";
            this.onWorkflowActivated1.Name = "onWorkflowActivated1";
            activitybind3.Name = "swfStratyZLatUbieglych";
            activitybind3.Path = "workflowProperties";
            this.onWorkflowActivated1.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.onWorkflowActivated1_Invoked);
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind4)));
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind3)));
            // 
            // swfStratyZLatUbieglych
            // 
            this.Activities.Add(this.onWorkflowActivated1);
            this.Activities.Add(this.sendInitiated);
            this.Activities.Add(this.Create_TargetList);
            this.Activities.Add(this.Get_ExistingKEYs);
            this.Activities.Add(this.whileCompleted);
            this.Activities.Add(this.sendCompleted);
            this.Activities.Add(this.logEnd);
            this.Name = "swfStratyZLatUbieglych";
            this.CanModifyActivities = false;

        }

        #endregion

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logEnd;

        private SequenceActivity Manage_Records;

        private WhileActivity whileCompleted;

        private CodeActivity Create_TargetList;

        private CodeActivity Append_Records;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity2;

        private IfElseBranchActivity Else;

        private IfElseBranchActivity ifRecordExist;

        private IfElseActivity SprawdzCzyIstnieje;

        private Microsoft.SharePoint.WorkflowActions.SendEmail sendCompleted;

        private CodeActivity Get_ExistingKEYs;

        private Microsoft.SharePoint.WorkflowActions.SendEmail sendInitiated;

        private Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated onWorkflowActivated1;




























    }
}
