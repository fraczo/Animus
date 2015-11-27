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

namespace Workflows.WyslijDoKlienta
{
    public sealed partial class WyslijDoKlienta
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
            System.Workflow.Activities.CodeCondition codecondition2 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Runtime.CorrelationToken correlationtoken1 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            this.Create_Message = new System.Workflow.Activities.CodeActivity();
            this.ifDaneDoWysylki2 = new System.Workflow.Activities.IfElseBranchActivity();
            this.Czy_są_dane_do_wysyłki = new System.Workflow.Activities.IfElseActivity();
            this.Create_RBR = new System.Workflow.Activities.CodeActivity();
            this.Create_VAT = new System.Workflow.Activities.CodeActivity();
            this.Create_PD = new System.Workflow.Activities.CodeActivity();
            this.Create_ZUSPD = new System.Workflow.Activities.CodeActivity();
            this.Create_ZUS = new System.Workflow.Activities.CodeActivity();
            this.STOP = new System.Workflow.ComponentModel.TerminateActivity();
            this.GenerujWiadomość = new System.Workflow.Activities.SequenceActivity();
            this.Manage_RBR = new System.Workflow.Activities.CodeActivity();
            this.Manage_VAT = new System.Workflow.Activities.CodeActivity();
            this.Manage_PD = new System.Workflow.Activities.CodeActivity();
            this.Manage_ZUSPD = new System.Workflow.Activities.CodeActivity();
            this.Manage_ZUS = new System.Workflow.Activities.CodeActivity();
            this.Else = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifDaneDoWysylki = new System.Workflow.Activities.IfElseBranchActivity();
            this.RBR = new System.Workflow.Activities.SequenceActivity();
            this.VAT = new System.Workflow.Activities.SequenceActivity();
            this.PD = new System.Workflow.Activities.SequenceActivity();
            this.ZUSPD = new System.Workflow.Activities.SequenceActivity();
            this.ZUS = new System.Workflow.Activities.SequenceActivity();
            this.Czy_są_zwolnione_do_wysyłki = new System.Workflow.Activities.IfElseActivity();
            this.Przygotowanie_komponentów_wiadomości = new System.Workflow.Activities.ParallelActivity();
            this.onWorkflowActivated1 = new Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated();
            // 
            // Create_Message
            // 
            this.Create_Message.Name = "Create_Message";
            this.Create_Message.ExecuteCode += new System.EventHandler(this.Create_Message_ExecuteCode);
            // 
            // ifDaneDoWysylki2
            // 
            this.ifDaneDoWysylki2.Activities.Add(this.Create_Message);
            codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.hasDaneDoWysylki);
            this.ifDaneDoWysylki2.Condition = codecondition1;
            this.ifDaneDoWysylki2.Name = "ifDaneDoWysylki2";
            // 
            // Czy_są_dane_do_wysyłki
            // 
            this.Czy_są_dane_do_wysyłki.Activities.Add(this.ifDaneDoWysylki2);
            this.Czy_są_dane_do_wysyłki.Name = "Czy_są_dane_do_wysyłki";
            // 
            // Create_RBR
            // 
            this.Create_RBR.Name = "Create_RBR";
            this.Create_RBR.ExecuteCode += new System.EventHandler(this.Create_RBR_ExecuteCode);
            // 
            // Create_VAT
            // 
            this.Create_VAT.Name = "Create_VAT";
            this.Create_VAT.ExecuteCode += new System.EventHandler(this.Create_VAT_ExecuteCode);
            // 
            // Create_PD
            // 
            this.Create_PD.Name = "Create_PD";
            this.Create_PD.ExecuteCode += new System.EventHandler(this.Create_PD_ExecuteCode);
            // 
            // Create_ZUSPD
            // 
            this.Create_ZUSPD.Name = "Create_ZUSPD";
            this.Create_ZUSPD.ExecuteCode += new System.EventHandler(this.Create_ZUSPD_ExecuteCode);
            // 
            // Create_ZUS
            // 
            this.Create_ZUS.Name = "Create_ZUS";
            this.Create_ZUS.ExecuteCode += new System.EventHandler(this.Create_ZUS_ExecuteCode);
            // 
            // STOP
            // 
            this.STOP.Name = "STOP";
            // 
            // GenerujWiadomość
            // 
            this.GenerujWiadomość.Activities.Add(this.Create_ZUS);
            this.GenerujWiadomość.Activities.Add(this.Create_ZUSPD);
            this.GenerujWiadomość.Activities.Add(this.Create_PD);
            this.GenerujWiadomość.Activities.Add(this.Create_VAT);
            this.GenerujWiadomość.Activities.Add(this.Create_RBR);
            this.GenerujWiadomość.Activities.Add(this.Czy_są_dane_do_wysyłki);
            this.GenerujWiadomość.Name = "GenerujWiadomość";
            // 
            // Manage_RBR
            // 
            this.Manage_RBR.Name = "Manage_RBR";
            this.Manage_RBR.ExecuteCode += new System.EventHandler(this.Manage_RBR_ExecuteCode);
            // 
            // Manage_VAT
            // 
            this.Manage_VAT.Name = "Manage_VAT";
            this.Manage_VAT.ExecuteCode += new System.EventHandler(this.Manage_VAT_ExecuteCode);
            // 
            // Manage_PD
            // 
            this.Manage_PD.Name = "Manage_PD";
            this.Manage_PD.ExecuteCode += new System.EventHandler(this.Manage_PD_ExecuteCode);
            // 
            // Manage_ZUSPD
            // 
            this.Manage_ZUSPD.Name = "Manage_ZUSPD";
            this.Manage_ZUSPD.ExecuteCode += new System.EventHandler(this.Manage_ZUSPD_ExecuteCode);
            // 
            // Manage_ZUS
            // 
            this.Manage_ZUS.Name = "Manage_ZUS";
            this.Manage_ZUS.ExecuteCode += new System.EventHandler(this.Manage_ZUS_ExecuteCode);
            // 
            // Else
            // 
            this.Else.Activities.Add(this.STOP);
            this.Else.Name = "Else";
            // 
            // ifDaneDoWysylki
            // 
            this.ifDaneDoWysylki.Activities.Add(this.GenerujWiadomość);
            codecondition2.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.hasDaneDoWysylki);
            this.ifDaneDoWysylki.Condition = codecondition2;
            this.ifDaneDoWysylki.Name = "ifDaneDoWysylki";
            // 
            // RBR
            // 
            this.RBR.Activities.Add(this.Manage_RBR);
            this.RBR.Name = "RBR";
            // 
            // VAT
            // 
            this.VAT.Activities.Add(this.Manage_VAT);
            this.VAT.Name = "VAT";
            // 
            // PD
            // 
            this.PD.Activities.Add(this.Manage_PD);
            this.PD.Name = "PD";
            // 
            // ZUSPD
            // 
            this.ZUSPD.Activities.Add(this.Manage_ZUSPD);
            this.ZUSPD.Name = "ZUSPD";
            // 
            // ZUS
            // 
            this.ZUS.Activities.Add(this.Manage_ZUS);
            this.ZUS.Name = "ZUS";
            // 
            // Czy_są_zwolnione_do_wysyłki
            // 
            this.Czy_są_zwolnione_do_wysyłki.Activities.Add(this.ifDaneDoWysylki);
            this.Czy_są_zwolnione_do_wysyłki.Activities.Add(this.Else);
            this.Czy_są_zwolnione_do_wysyłki.Name = "Czy_są_zwolnione_do_wysyłki";
            // 
            // Przygotowanie_komponentów_wiadomości
            // 
            this.Przygotowanie_komponentów_wiadomości.Activities.Add(this.ZUS);
            this.Przygotowanie_komponentów_wiadomości.Activities.Add(this.ZUSPD);
            this.Przygotowanie_komponentów_wiadomości.Activities.Add(this.PD);
            this.Przygotowanie_komponentów_wiadomości.Activities.Add(this.VAT);
            this.Przygotowanie_komponentów_wiadomości.Activities.Add(this.RBR);
            this.Przygotowanie_komponentów_wiadomości.Name = "Przygotowanie_komponentów_wiadomości";
            activitybind2.Name = "WyslijDoKlienta";
            activitybind2.Path = "workflowId";
            // 
            // onWorkflowActivated1
            // 
            correlationtoken1.Name = "workflowToken";
            correlationtoken1.OwnerActivityName = "WyslijDoKlienta";
            this.onWorkflowActivated1.CorrelationToken = correlationtoken1;
            this.onWorkflowActivated1.EventName = "OnWorkflowActivated";
            this.onWorkflowActivated1.Name = "onWorkflowActivated1";
            activitybind1.Name = "WyslijDoKlienta";
            activitybind1.Path = "workflowProperties";
            this.onWorkflowActivated1.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.onWorkflowActivated1_Invoked);
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            // 
            // WyslijDoKlienta
            // 
            this.Activities.Add(this.onWorkflowActivated1);
            this.Activities.Add(this.Przygotowanie_komponentów_wiadomości);
            this.Activities.Add(this.Czy_są_zwolnione_do_wysyłki);
            this.Name = "WyslijDoKlienta";
            this.CanModifyActivities = false;

        }

        #endregion

        private CodeActivity Create_Message;

        private IfElseBranchActivity ifDaneDoWysylki2;

        private IfElseActivity Czy_są_dane_do_wysyłki;

        private CodeActivity Create_RBR;

        private CodeActivity Create_VAT;

        private CodeActivity Create_PD;

        private CodeActivity Create_ZUSPD;

        private CodeActivity Create_ZUS;

        private TerminateActivity STOP;

        private SequenceActivity GenerujWiadomość;

        private CodeActivity Manage_RBR;

        private CodeActivity Manage_VAT;

        private CodeActivity Manage_PD;

        private CodeActivity Manage_ZUSPD;

        private CodeActivity Manage_ZUS;

        private IfElseBranchActivity Else;

        private IfElseBranchActivity ifDaneDoWysylki;

        private SequenceActivity RBR;

        private SequenceActivity VAT;

        private SequenceActivity PD;

        private SequenceActivity ZUSPD;

        private SequenceActivity ZUS;

        private IfElseActivity Czy_są_zwolnione_do_wysyłki;

        private ParallelActivity Przygotowanie_komponentów_wiadomości;

        private Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated onWorkflowActivated1;














    }
}
