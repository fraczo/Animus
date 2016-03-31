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

namespace Workflows.ObslugaWiadomosci
{
    public sealed partial class ObslugaWiadomosci
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
            System.Workflow.Activities.CodeCondition codecondition3 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.Activities.CodeCondition codecondition4 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Runtime.CorrelationToken correlationtoken1 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            this.ReportError_ZadaniePrzezKK = new System.Workflow.Activities.CodeActivity();
            this.ReportError_Zadanie = new System.Workflow.Activities.CodeActivity();
            this.faultHandlerActivity1 = new System.Workflow.ComponentModel.FaultHandlerActivity();
            this.faultHandlerActivity2 = new System.Workflow.ComponentModel.FaultHandlerActivity();
            this.logToHistoryListActivity9 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.faultHandlersActivity2 = new System.Workflow.ComponentModel.FaultHandlersActivity();
            this.Update_KK_Zadania = new System.Workflow.Activities.CodeActivity();
            this.logToHistoryListActivity7 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.faultHandlersActivity3 = new System.Workflow.ComponentModel.FaultHandlersActivity();
            this.Update_Zadanie = new System.Workflow.Activities.CodeActivity();
            this.logToHistoryListActivity5 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.Else4 = new System.Workflow.Activities.IfElseBranchActivity();
            this.Zadanie_przez_KK = new System.Workflow.Activities.IfElseBranchActivity();
            this.Zadanie_bezposrednio = new System.Workflow.Activities.IfElseBranchActivity();
            this.logNie_wysłana = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.ifElseActivity1 = new System.Workflow.Activities.IfElseActivity();
            this.logWysłana = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.Else3 = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifMessageSent = new System.Workflow.Activities.IfElseBranchActivity();
            this.ReportError = new System.Workflow.Activities.CodeActivity();
            this.Jeżeli_wyłana = new System.Workflow.Activities.IfElseActivity();
            this.Mail_Send = new System.Workflow.Activities.CodeActivity();
            this.logToHistoryListActivity4 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.Mail_Setup = new System.Workflow.Activities.CodeActivity();
            this.logToHistoryListActivity3 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.logToHistoryListActivity1 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.faultObslugaWiadomosci = new System.Workflow.ComponentModel.FaultHandlerActivity();
            this.Else = new System.Workflow.Activities.IfElseBranchActivity();
            this.isMailSent = new System.Workflow.Activities.IfElseBranchActivity();
            this.faultHandlersActivity1 = new System.Workflow.ComponentModel.FaultHandlersActivity();
            this.cancellationHandlerActivity1 = new System.Workflow.ComponentModel.CancellationHandlerActivity();
            this.logEnd = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.CzyWiadomośćWysłana = new System.Workflow.Activities.IfElseActivity();
            this.logToHistoryListActivity2 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.Ignoruj_status_wysyłki = new System.Workflow.Activities.CodeActivity();
            this.onWorkflowActivated1 = new Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated();
            // 
            // ReportError_ZadaniePrzezKK
            // 
            this.ReportError_ZadaniePrzezKK.Name = "ReportError_ZadaniePrzezKK";
            this.ReportError_ZadaniePrzezKK.ExecuteCode += new System.EventHandler(this.ReportError_ExecuteCode);
            // 
            // ReportError_Zadanie
            // 
            this.ReportError_Zadanie.Name = "ReportError_Zadanie";
            this.ReportError_Zadanie.ExecuteCode += new System.EventHandler(this.ReportError_ExecuteCode);
            // 
            // faultHandlerActivity1
            // 
            this.faultHandlerActivity1.Activities.Add(this.ReportError_ZadaniePrzezKK);
            this.faultHandlerActivity1.FaultType = typeof(System.Exception);
            this.faultHandlerActivity1.Name = "faultHandlerActivity1";
            // 
            // faultHandlerActivity2
            // 
            this.faultHandlerActivity2.Activities.Add(this.ReportError_Zadanie);
            this.faultHandlerActivity2.FaultType = typeof(System.Exception);
            this.faultHandlerActivity2.Name = "faultHandlerActivity2";
            // 
            // logToHistoryListActivity9
            // 
            this.logToHistoryListActivity9.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity9.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity9.HistoryDescription = "Brak powiązanych kartotek do obsługi";
            this.logToHistoryListActivity9.HistoryOutcome = "";
            this.logToHistoryListActivity9.Name = "logToHistoryListActivity9";
            this.logToHistoryListActivity9.OtherData = "";
            this.logToHistoryListActivity9.UserId = -1;
            // 
            // faultHandlersActivity2
            // 
            this.faultHandlersActivity2.Activities.Add(this.faultHandlerActivity1);
            this.faultHandlersActivity2.Name = "faultHandlersActivity2";
            // 
            // Update_KK_Zadania
            // 
            this.Update_KK_Zadania.Name = "Update_KK_Zadania";
            this.Update_KK_Zadania.ExecuteCode += new System.EventHandler(this.Update_KK_Zadania_ExecuteCode);
            // 
            // logToHistoryListActivity7
            // 
            this.logToHistoryListActivity7.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity7.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity7.HistoryDescription = "Aktualizacja kart kontrolnych i zadań powiązanych";
            this.logToHistoryListActivity7.HistoryOutcome = "";
            this.logToHistoryListActivity7.Name = "logToHistoryListActivity7";
            this.logToHistoryListActivity7.OtherData = "";
            this.logToHistoryListActivity7.UserId = -1;
            // 
            // faultHandlersActivity3
            // 
            this.faultHandlersActivity3.Activities.Add(this.faultHandlerActivity2);
            this.faultHandlersActivity3.Name = "faultHandlersActivity3";
            // 
            // Update_Zadanie
            // 
            this.Update_Zadanie.Name = "Update_Zadanie";
            this.Update_Zadanie.ExecuteCode += new System.EventHandler(this.Update_Zadanie_ExecuteCode);
            // 
            // logToHistoryListActivity5
            // 
            this.logToHistoryListActivity5.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity5.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity5.HistoryDescription = "Aktualizacja zadania";
            this.logToHistoryListActivity5.HistoryOutcome = "";
            this.logToHistoryListActivity5.Name = "logToHistoryListActivity5";
            this.logToHistoryListActivity5.OtherData = "";
            this.logToHistoryListActivity5.UserId = -1;
            // 
            // Else4
            // 
            this.Else4.Activities.Add(this.logToHistoryListActivity9);
            this.Else4.Name = "Else4";
            // 
            // Zadanie_przez_KK
            // 
            this.Zadanie_przez_KK.Activities.Add(this.logToHistoryListActivity7);
            this.Zadanie_przez_KK.Activities.Add(this.Update_KK_Zadania);
            this.Zadanie_przez_KK.Activities.Add(this.faultHandlersActivity2);
            codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.isKartaKontrolnaToBeUpdated);
            this.Zadanie_przez_KK.Condition = codecondition1;
            this.Zadanie_przez_KK.Name = "Zadanie_przez_KK";
            // 
            // Zadanie_bezposrednio
            // 
            this.Zadanie_bezposrednio.Activities.Add(this.logToHistoryListActivity5);
            this.Zadanie_bezposrednio.Activities.Add(this.Update_Zadanie);
            this.Zadanie_bezposrednio.Activities.Add(this.faultHandlersActivity3);
            codecondition2.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.IsZadanieToBeUpdated);
            this.Zadanie_bezposrednio.Condition = codecondition2;
            this.Zadanie_bezposrednio.Name = "Zadanie_bezposrednio";
            // 
            // logNie_wysłana
            // 
            this.logNie_wysłana.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logNie_wysłana.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logNie_wysłana.HistoryDescription = "Wiadomość NIE wysłana";
            this.logNie_wysłana.HistoryOutcome = "";
            this.logNie_wysłana.Name = "logNie_wysłana";
            this.logNie_wysłana.OtherData = "";
            this.logNie_wysłana.UserId = -1;
            // 
            // ifElseActivity1
            // 
            this.ifElseActivity1.Activities.Add(this.Zadanie_bezposrednio);
            this.ifElseActivity1.Activities.Add(this.Zadanie_przez_KK);
            this.ifElseActivity1.Activities.Add(this.Else4);
            this.ifElseActivity1.Name = "ifElseActivity1";
            // 
            // logWysłana
            // 
            this.logWysłana.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logWysłana.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logWysłana.HistoryDescription = "Wiadomość wysłana";
            this.logWysłana.HistoryOutcome = "";
            this.logWysłana.Name = "logWysłana";
            this.logWysłana.OtherData = "";
            this.logWysłana.UserId = -1;
            // 
            // Else3
            // 
            this.Else3.Activities.Add(this.logNie_wysłana);
            this.Else3.Name = "Else3";
            // 
            // ifMessageSent
            // 
            this.ifMessageSent.Activities.Add(this.logWysłana);
            this.ifMessageSent.Activities.Add(this.ifElseActivity1);
            codecondition3.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.isWiadomoscWyslana);
            this.ifMessageSent.Condition = codecondition3;
            this.ifMessageSent.Name = "ifMessageSent";
            // 
            // ReportError
            // 
            this.ReportError.Name = "ReportError";
            this.ReportError.ExecuteCode += new System.EventHandler(this.ReportError_ExecuteCode);
            // 
            // Jeżeli_wyłana
            // 
            this.Jeżeli_wyłana.Activities.Add(this.ifMessageSent);
            this.Jeżeli_wyłana.Activities.Add(this.Else3);
            this.Jeżeli_wyłana.Name = "Jeżeli_wyłana";
            // 
            // Mail_Send
            // 
            this.Mail_Send.Name = "Mail_Send";
            this.Mail_Send.ExecuteCode += new System.EventHandler(this.Mail_Send_ExecuteCode);
            // 
            // logToHistoryListActivity4
            // 
            this.logToHistoryListActivity4.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity4.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity4.HistoryDescription = "Zlecenie wysyłki";
            this.logToHistoryListActivity4.HistoryOutcome = "";
            this.logToHistoryListActivity4.Name = "logToHistoryListActivity4";
            this.logToHistoryListActivity4.OtherData = "";
            this.logToHistoryListActivity4.UserId = -1;
            // 
            // Mail_Setup
            // 
            this.Mail_Setup.Name = "Mail_Setup";
            this.Mail_Setup.ExecuteCode += new System.EventHandler(this.Mail_Setup_ExecuteCode);
            // 
            // logToHistoryListActivity3
            // 
            this.logToHistoryListActivity3.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity3.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity3.HistoryDescription = "Przygotowanie";
            this.logToHistoryListActivity3.HistoryOutcome = "";
            this.logToHistoryListActivity3.Name = "logToHistoryListActivity3";
            this.logToHistoryListActivity3.OtherData = "";
            this.logToHistoryListActivity3.UserId = -1;
            // 
            // logToHistoryListActivity1
            // 
            this.logToHistoryListActivity1.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity1.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity1.HistoryDescription = "STOP: wiadomość oznaczona jako wysłana";
            this.logToHistoryListActivity1.HistoryOutcome = "";
            this.logToHistoryListActivity1.Name = "logToHistoryListActivity1";
            this.logToHistoryListActivity1.OtherData = "";
            this.logToHistoryListActivity1.UserId = -1;
            // 
            // faultObslugaWiadomosci
            // 
            this.faultObslugaWiadomosci.Activities.Add(this.ReportError);
            this.faultObslugaWiadomosci.FaultType = typeof(System.Exception);
            this.faultObslugaWiadomosci.Name = "faultObslugaWiadomosci";
            // 
            // Else
            // 
            this.Else.Activities.Add(this.logToHistoryListActivity3);
            this.Else.Activities.Add(this.Mail_Setup);
            this.Else.Activities.Add(this.logToHistoryListActivity4);
            this.Else.Activities.Add(this.Mail_Send);
            this.Else.Activities.Add(this.Jeżeli_wyłana);
            this.Else.Name = "Else";
            // 
            // isMailSent
            // 
            this.isMailSent.Activities.Add(this.logToHistoryListActivity1);
            codecondition4.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.isFlagaWysłanoUstawiona);
            this.isMailSent.Condition = codecondition4;
            this.isMailSent.Name = "isMailSent";
            // 
            // faultHandlersActivity1
            // 
            this.faultHandlersActivity1.Activities.Add(this.faultObslugaWiadomosci);
            this.faultHandlersActivity1.Name = "faultHandlersActivity1";
            // 
            // cancellationHandlerActivity1
            // 
            this.cancellationHandlerActivity1.Name = "cancellationHandlerActivity1";
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
            // CzyWiadomośćWysłana
            // 
            this.CzyWiadomośćWysłana.Activities.Add(this.isMailSent);
            this.CzyWiadomośćWysłana.Activities.Add(this.Else);
            this.CzyWiadomośćWysłana.Name = "CzyWiadomośćWysłana";
            // 
            // logToHistoryListActivity2
            // 
            this.logToHistoryListActivity2.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity2.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity2.HistoryDescription = "Obsługa";
            this.logToHistoryListActivity2.HistoryOutcome = "";
            this.logToHistoryListActivity2.Name = "logToHistoryListActivity2";
            this.logToHistoryListActivity2.OtherData = "";
            this.logToHistoryListActivity2.UserId = -1;
            // 
            // Ignoruj_status_wysyłki
            // 
            this.Ignoruj_status_wysyłki.Enabled = false;
            this.Ignoruj_status_wysyłki.Name = "Ignoruj_status_wysyłki";
            this.Ignoruj_status_wysyłki.ExecuteCode += new System.EventHandler(this.codeActivity1_ExecuteCode);
            activitybind2.Name = "ObslugaWiadomosci";
            activitybind2.Path = "workflowId";
            // 
            // onWorkflowActivated1
            // 
            correlationtoken1.Name = "workflowToken";
            correlationtoken1.OwnerActivityName = "ObslugaWiadomosci";
            this.onWorkflowActivated1.CorrelationToken = correlationtoken1;
            this.onWorkflowActivated1.EventName = "OnWorkflowActivated";
            this.onWorkflowActivated1.Name = "onWorkflowActivated1";
            activitybind1.Name = "ObslugaWiadomosci";
            activitybind1.Path = "workflowProperties";
            this.onWorkflowActivated1.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.onWorkflowActivated1_Invoked);
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            // 
            // ObslugaWiadomosci
            // 
            this.Activities.Add(this.onWorkflowActivated1);
            this.Activities.Add(this.Ignoruj_status_wysyłki);
            this.Activities.Add(this.logToHistoryListActivity2);
            this.Activities.Add(this.CzyWiadomośćWysłana);
            this.Activities.Add(this.logEnd);
            this.Activities.Add(this.cancellationHandlerActivity1);
            this.Activities.Add(this.faultHandlersActivity1);
            this.Name = "ObslugaWiadomosci";
            this.CanModifyActivities = false;

        }

        #endregion

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logEnd;

        private CodeActivity ReportError_Zadanie;

        private FaultHandlerActivity faultHandlerActivity2;

        private FaultHandlersActivity faultHandlersActivity3;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity9;

        private IfElseBranchActivity Else4;

        private CodeActivity ReportError_ZadaniePrzezKK;

        private FaultHandlerActivity faultHandlerActivity1;

        private FaultHandlersActivity faultHandlersActivity2;

        private CodeActivity Update_KK_Zadania;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity7;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity5;

        private IfElseBranchActivity Zadanie_przez_KK;

        private IfElseBranchActivity Zadanie_bezposrednio;

        private IfElseActivity ifElseActivity1;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logNie_wysłana;

        private IfElseBranchActivity Else3;

        private IfElseBranchActivity ifMessageSent;

        private IfElseActivity Jeżeli_wyłana;

        private CodeActivity Ignoruj_status_wysyłki;

        private CodeActivity ReportError;

        private FaultHandlerActivity faultObslugaWiadomosci;

        private FaultHandlersActivity faultHandlersActivity1;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logWysłana;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity4;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity3;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity2;

        private CodeActivity Mail_Setup;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity1;

        private IfElseBranchActivity Else;

        private CancellationHandlerActivity cancellationHandlerActivity1;

        private CodeActivity Update_Zadanie;

        private CodeActivity Mail_Send;

        private IfElseBranchActivity isMailSent;

        private IfElseActivity CzyWiadomośćWysłana;

        private Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated onWorkflowActivated1;












































































































    }
}
