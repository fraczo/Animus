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

namespace Workflows.PrzygotujWiadomosc
{
    public sealed partial class PrzygotujWiadomosc
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
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Activities.CodeCondition codecondition3 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.ComponentModel.ActivityBind activitybind3 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Runtime.CorrelationToken correlationtoken1 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            this.logStatusyZadania = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.Update_StatusyZadan = new System.Workflow.Activities.CodeActivity();
            this.logStatusyKK = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.Update_StatusyKK = new System.Workflow.Activities.CodeActivity();
            this.ifMessage_Created = new System.Workflow.Activities.IfElseBranchActivity();
            this.Jeżeli_wiadomość_utworzna = new System.Workflow.Activities.IfElseActivity();
            this.logWiadomoscPrzygotowana = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.Create_Message = new System.Workflow.Activities.CodeActivity();
            this.Create_Body = new System.Workflow.Activities.CodeActivity();
            this.Create_Footer = new System.Workflow.Activities.CodeActivity();
            this.logIstniejaInformacjeDoWyslania = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.ifDaneDoWysylki2 = new System.Workflow.Activities.IfElseBranchActivity();
            this.Czy_są_dane_do_wysyłki = new System.Workflow.Activities.IfElseActivity();
            this.Create_RBR = new System.Workflow.Activities.CodeActivity();
            this.Create_VAT = new System.Workflow.Activities.CodeActivity();
            this.Create_PD = new System.Workflow.Activities.CodeActivity();
            this.Create_ZUSPD = new System.Workflow.Activities.CodeActivity();
            this.Create_ZUS = new System.Workflow.Activities.CodeActivity();
            this.logErrorMessage = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.FaultHandler = new System.Workflow.Activities.CodeActivity();
            this.STOP = new System.Workflow.ComponentModel.TerminateActivity();
            this.logBrakInformacjiDoWyslania = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.GenerujWiadomość = new System.Workflow.Activities.SequenceActivity();
            this.Manage_RBR = new System.Workflow.Activities.CodeActivity();
            this.Manage_VAT = new System.Workflow.Activities.CodeActivity();
            this.Manage_PD = new System.Workflow.Activities.CodeActivity();
            this.Manage_ZUSPD = new System.Workflow.Activities.CodeActivity();
            this.Manage_ZUS = new System.Workflow.Activities.CodeActivity();
            this.faultHandlerActivity1 = new System.Workflow.ComponentModel.FaultHandlerActivity();
            this.Else = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifDaneDoWysylki = new System.Workflow.Activities.IfElseBranchActivity();
            this.RBR = new System.Workflow.Activities.SequenceActivity();
            this.VAT = new System.Workflow.Activities.SequenceActivity();
            this.PD = new System.Workflow.Activities.SequenceActivity();
            this.ZUSPD = new System.Workflow.Activities.SequenceActivity();
            this.ZUS = new System.Workflow.Activities.SequenceActivity();
            this.cancellationHandlerActivity1 = new System.Workflow.ComponentModel.CancellationHandlerActivity();
            this.faultHandlersActivity1 = new System.Workflow.ComponentModel.FaultHandlersActivity();
            this.Czy_są_zwolnione_do_wysyłki = new System.Workflow.Activities.IfElseActivity();
            this.Przygotowanie_komponentów_wiadomości = new System.Workflow.Activities.ParallelActivity();
            this.onWorkflowActivated1 = new Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated();
            // 
            // logStatusyZadania
            // 
            this.logStatusyZadania.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logStatusyZadania.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logStatusyZadania.HistoryDescription = "Statusy w zadaniach zaktualizowane";
            this.logStatusyZadania.HistoryOutcome = "";
            this.logStatusyZadania.Name = "logStatusyZadania";
            this.logStatusyZadania.OtherData = "";
            this.logStatusyZadania.UserId = -1;
            // 
            // Update_StatusyZadan
            // 
            this.Update_StatusyZadan.Name = "Update_StatusyZadan";
            this.Update_StatusyZadan.ExecuteCode += new System.EventHandler(this.Update_StatusyZadan_ExecuteCode);
            // 
            // logStatusyKK
            // 
            this.logStatusyKK.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logStatusyKK.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logStatusyKK.HistoryDescription = "Statusy na kartach kontrolnych zaktualizowane";
            this.logStatusyKK.HistoryOutcome = "";
            this.logStatusyKK.Name = "logStatusyKK";
            this.logStatusyKK.OtherData = "";
            this.logStatusyKK.UserId = -1;
            // 
            // Update_StatusyKK
            // 
            this.Update_StatusyKK.Name = "Update_StatusyKK";
            this.Update_StatusyKK.ExecuteCode += new System.EventHandler(this.Update_StatusyKK_ExecuteCode);
            // 
            // ifMessage_Created
            // 
            this.ifMessage_Created.Activities.Add(this.Update_StatusyKK);
            this.ifMessage_Created.Activities.Add(this.logStatusyKK);
            this.ifMessage_Created.Activities.Add(this.Update_StatusyZadan);
            this.ifMessage_Created.Activities.Add(this.logStatusyZadania);
            codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.isMessage_Created);
            this.ifMessage_Created.Condition = codecondition1;
            this.ifMessage_Created.Name = "ifMessage_Created";
            // 
            // Jeżeli_wiadomość_utworzna
            // 
            this.Jeżeli_wiadomość_utworzna.Activities.Add(this.ifMessage_Created);
            this.Jeżeli_wiadomość_utworzna.Name = "Jeżeli_wiadomość_utworzna";
            // 
            // logWiadomoscPrzygotowana
            // 
            this.logWiadomoscPrzygotowana.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logWiadomoscPrzygotowana.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logWiadomoscPrzygotowana.HistoryDescription = "Nowa wiadomość wygenerowana";
            this.logWiadomoscPrzygotowana.HistoryOutcome = "";
            this.logWiadomoscPrzygotowana.Name = "logWiadomoscPrzygotowana";
            this.logWiadomoscPrzygotowana.OtherData = "";
            this.logWiadomoscPrzygotowana.UserId = -1;
            // 
            // Create_Message
            // 
            this.Create_Message.Name = "Create_Message";
            this.Create_Message.ExecuteCode += new System.EventHandler(this.Create_Message_ExecuteCode);
            // 
            // Create_Body
            // 
            this.Create_Body.Name = "Create_Body";
            this.Create_Body.ExecuteCode += new System.EventHandler(this.Create_Body_ExecuteCode);
            // 
            // Create_Footer
            // 
            this.Create_Footer.Name = "Create_Footer";
            this.Create_Footer.ExecuteCode += new System.EventHandler(this.Create_Footer_ExecuteCode);
            // 
            // logIstniejaInformacjeDoWyslania
            // 
            this.logIstniejaInformacjeDoWyslania.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logIstniejaInformacjeDoWyslania.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logIstniejaInformacjeDoWyslania.HistoryDescription = "Znaleziono informacje do wysyłania";
            this.logIstniejaInformacjeDoWyslania.HistoryOutcome = "";
            this.logIstniejaInformacjeDoWyslania.Name = "logIstniejaInformacjeDoWyslania";
            this.logIstniejaInformacjeDoWyslania.OtherData = "";
            this.logIstniejaInformacjeDoWyslania.UserId = -1;
            // 
            // ifDaneDoWysylki2
            // 
            this.ifDaneDoWysylki2.Activities.Add(this.logIstniejaInformacjeDoWyslania);
            this.ifDaneDoWysylki2.Activities.Add(this.Create_Footer);
            this.ifDaneDoWysylki2.Activities.Add(this.Create_Body);
            this.ifDaneDoWysylki2.Activities.Add(this.Create_Message);
            this.ifDaneDoWysylki2.Activities.Add(this.logWiadomoscPrzygotowana);
            this.ifDaneDoWysylki2.Activities.Add(this.Jeżeli_wiadomość_utworzna);
            codecondition2.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.hasDaneDoWysylki);
            this.ifDaneDoWysylki2.Condition = codecondition2;
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
            // logErrorMessage
            // 
            this.logErrorMessage.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logErrorMessage.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            activitybind1.Name = "PrzygotujWiadomosc";
            activitybind1.Path = "logErrorMessage_HistoryDescription";
            this.logErrorMessage.HistoryOutcome = "";
            this.logErrorMessage.Name = "logErrorMessage";
            this.logErrorMessage.OtherData = "";
            this.logErrorMessage.UserId = -1;
            this.logErrorMessage.SetBinding(Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity.HistoryDescriptionProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            // 
            // FaultHandler
            // 
            this.FaultHandler.Name = "FaultHandler";
            this.FaultHandler.ExecuteCode += new System.EventHandler(this.FaultHandler_ExecuteCode);
            // 
            // STOP
            // 
            this.STOP.Name = "STOP";
            // 
            // logBrakInformacjiDoWyslania
            // 
            this.logBrakInformacjiDoWyslania.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logBrakInformacjiDoWyslania.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logBrakInformacjiDoWyslania.HistoryDescription = "Brak informacji do wysłania";
            this.logBrakInformacjiDoWyslania.HistoryOutcome = "";
            this.logBrakInformacjiDoWyslania.Name = "logBrakInformacjiDoWyslania";
            this.logBrakInformacjiDoWyslania.OtherData = "";
            this.logBrakInformacjiDoWyslania.UserId = -1;
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
            // faultHandlerActivity1
            // 
            this.faultHandlerActivity1.Activities.Add(this.FaultHandler);
            this.faultHandlerActivity1.Activities.Add(this.logErrorMessage);
            this.faultHandlerActivity1.FaultType = typeof(System.Exception);
            this.faultHandlerActivity1.Name = "faultHandlerActivity1";
            // 
            // Else
            // 
            this.Else.Activities.Add(this.logBrakInformacjiDoWyslania);
            this.Else.Activities.Add(this.STOP);
            this.Else.Name = "Else";
            // 
            // ifDaneDoWysylki
            // 
            this.ifDaneDoWysylki.Activities.Add(this.GenerujWiadomość);
            codecondition3.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.hasDaneDoWysylki);
            this.ifDaneDoWysylki.Condition = codecondition3;
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
            // cancellationHandlerActivity1
            // 
            this.cancellationHandlerActivity1.Name = "cancellationHandlerActivity1";
            // 
            // faultHandlersActivity1
            // 
            this.faultHandlersActivity1.Activities.Add(this.faultHandlerActivity1);
            this.faultHandlersActivity1.Name = "faultHandlersActivity1";
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
            activitybind3.Name = "PrzygotujWiadomosc";
            activitybind3.Path = "workflowId";
            // 
            // onWorkflowActivated1
            // 
            correlationtoken1.Name = "workflowToken";
            correlationtoken1.OwnerActivityName = "PrzygotujWiadomosc";
            this.onWorkflowActivated1.CorrelationToken = correlationtoken1;
            this.onWorkflowActivated1.EventName = "OnWorkflowActivated";
            this.onWorkflowActivated1.Name = "onWorkflowActivated1";
            activitybind2.Name = "PrzygotujWiadomosc";
            activitybind2.Path = "workflowProperties";
            this.onWorkflowActivated1.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.onWorkflowActivated1_Invoked);
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind3)));
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            // 
            // PrzygotujWiadomosc
            // 
            this.Activities.Add(this.onWorkflowActivated1);
            this.Activities.Add(this.Przygotowanie_komponentów_wiadomości);
            this.Activities.Add(this.Czy_są_zwolnione_do_wysyłki);
            this.Activities.Add(this.faultHandlersActivity1);
            this.Activities.Add(this.cancellationHandlerActivity1);
            this.Name = "PrzygotujWiadomosc";
            this.CanModifyActivities = false;

        }

        #endregion

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logStatusyZadania;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logStatusyKK;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logIstniejaInformacjeDoWyslania;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logBrakInformacjiDoWyslania;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logWiadomoscPrzygotowana;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logErrorMessage;

        private CodeActivity Update_StatusyKK;

        private CodeActivity Update_StatusyZadan;

        private IfElseBranchActivity ifMessage_Created;

        private IfElseActivity Jeżeli_wiadomość_utworzna;

        private CodeActivity Create_Footer;

        private CodeActivity Create_Body;

        private CancellationHandlerActivity cancellationHandlerActivity1;

        private CodeActivity FaultHandler;

        private FaultHandlerActivity faultHandlerActivity1;

        private FaultHandlersActivity faultHandlersActivity1;

        private CodeActivity Create_Message;

        private IfElseBranchActivity ifDaneDoWysylki2;

        private IfElseActivity Czy_są_dane_do_wysyłki;

        private CodeActivity Create_RBR;

        private CodeActivity Create_PD;

        private CodeActivity Create_ZUSPD;

        private CodeActivity Create_ZUS;

        private SequenceActivity GenerujWiadomość;

        private CodeActivity Create_VAT;

        private CodeActivity Manage_RBR;

        private CodeActivity Manage_VAT;

        private CodeActivity Manage_PD;

        private CodeActivity Manage_ZUSPD;

        private CodeActivity Manage_ZUS;

        private SequenceActivity RBR;

        private SequenceActivity VAT;

        private SequenceActivity PD;

        private SequenceActivity ZUSPD;

        private SequenceActivity ZUS;

        private ParallelActivity Przygotowanie_komponentów_wiadomości;

        private TerminateActivity STOP;

        private IfElseBranchActivity Else;

        private IfElseBranchActivity ifDaneDoWysylki;

        private IfElseActivity Czy_są_zwolnione_do_wysyłki;

        private Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated onWorkflowActivated1;































    }
}
