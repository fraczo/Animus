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
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Workflows.PrzygotujWiadomosc
{
    public sealed partial class PrzygotujWiadomosc : SequentialWorkflowActivity
    {
        public PrzygotujWiadomosc()
        {
            InitializeComponent();
        }

        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();
        private SPListItem item;
        private BLL.Models.Klient iok;

        private StringBuilder sbZUS;
        private StringBuilder sbZUSPD;
        private StringBuilder sbPD;
        private StringBuilder sbVAT;
        private StringBuilder sbRBR;

        private StringBuilder sbBody;
        private StringBuilder sbFooter;

        public String logErrorMessage_HistoryDescription = default(System.String);
        private string mailSubject;
        private string mailTo;
        private string mailFrom;
        private string mailCC;

        private const string _ZADANIE_ZWOLNIONE = "Zwolnione do wysyłki";

        private const string _ZUS_HTML_TEMPLATE_NAME = "ZUS_TEMPLATE";
        private const string _ZUSPD_HTML_TEMPLATE_NAME = "ZUSPD_TEMPLATE";
        private const string _PD_HTML_TEMPLATE_NAME = "PD_TEMPLATE";
        private const string _VAT_HTML_TEMPLATE_NAME = "VAT_TEMPLATE";
        private const string _RBR_HTML_TEMPLATE_NAME = "RBR_TEMPLATE";

        private string _FOOTER_HTML_TEMPLATE_NAME = "FOOTER_TEMPLATE";
        private string _BODY_HTML_TEMPLATE_NAME = "BODY_TEMPLATE";
        private bool messageCreated = false;

        private string _ZUS_TYTUL = "ZUS - Wysokość składek do zapłaty";
        private string _ZUSPD_TYTUL = "Urząd Skarbowy - Podatek za pracowników do zapłaty";
        private string _PD_TYTUL = "Urząd Skarbowy - Podatek dochodowy do zapłaty";
        private string _PD_TYTUL_STRATA = "Urząd Skarbowy - Informacja o poniesionej stracie";
        private string _VAT_TYTUL = "Urząd Skarbowy - Podatek VAT do zapłaty";
        private string _VAT_TYTUL_PRZENIESIENIE = "Urząd Skarbowy - Podatek VAT do przeniesienia";
        private string _VAT_TYTUL_ZWROT = "Urząd Skarbowy - Podatek VAT do zwrotu";
        private string _VAT_TYTUL_PRZENIESIENIE_ZWROT = "Urząd Skarbowy - Podatek VAT do przeniesienia i do zwrotu";
        private string _RBR_TYTUL = "Rozliczenie z biurem rachunkowym";
        private string mailBCC;
        private int selectedOperatorId;
        private string _STATUS_ZADANIA_WYSYLKA = "Wysyłka";


        #region Helpers

        internal bool HasStatus(SPListItem item, string col, string status)
        {
            if (BLL.Tools.Get_Text(item, col).Equals(status)) return true;
            else return false;
        }

        private void ReplaceCurrency(StringBuilder sb, SPListItem item, string col)
        {
            double v = BLL.Tools.Get_Value(item, col);
            sb.Replace(string.Format(@"___{0}___", col), BLL.Tools.Format_Currency(v));
            sb.Replace(string.Format(@"[[{0}]]", col), BLL.Tools.Format_Currency(v));
        }

        private void ReplaceString(StringBuilder sb, SPListItem item, string col)
        {
            string s = BLL.Tools.Get_Text(item, col);
            sb.Replace(string.Format(@"___{0}___", col), s);
            sb.Replace(string.Format(@"[[{0}]]", col), s);
        }

        private void ReplaceString(StringBuilder sb, string col, string s)
        {
            sb.Replace(string.Format(@"___{0}___", col), s);
            sb.Replace(string.Format(@"[[{0}]]", col), s);
        }

        private void ReplaceDate(StringBuilder sb, SPListItem item, string col)
        {
            DateTime d = BLL.Tools.Get_Date(item, col);
            sb.Replace(string.Format(@"___{0}___", col), BLL.Tools.Format_Date(d));
            sb.Replace(string.Format(@"[[{0}]]", col), BLL.Tools.Format_Date(d));
        }

        private void ReplaceDate(StringBuilder sb, SPListItem item, string col, DateTime d)
        {
            sb.Replace(string.Format(@"___{0}___", col), BLL.Tools.Format_Date(d));
            sb.Replace(string.Format(@"[[{0}]]", col), BLL.Tools.Format_Date(d));
        }

        private void FaultHandler_ExecuteCode(object sender, EventArgs e)
        {
            FaultHandlerActivity fa = ((Activity)sender).Parent as FaultHandlerActivity;
            if (fa != null)
            {
                //BLL.Logger.LogError(fa.Fault.Source, fa.Fault.Message + " " + fa.Fault.StackTrace, null);

                Debug.WriteLine(fa.Fault.Source);
                Debug.WriteLine(fa.Fault.Message);
                Debug.WriteLine(fa.Fault.StackTrace);

                logErrorMessage_HistoryDescription = string.Format("{0}::{1}",
                    fa.Fault.Message,
                    fa.Fault.StackTrace);
            }

        }

        private void AppendSeparator(StringBuilder sb, int sCounter)
        {
            if (sCounter > 0) sb.Append(@"<hr noshade=""noshade"" style=""color: #F4F4F4; height: 1px"">");
        }

        private string Format_IdPlat(string s)
        {
            return string.Format(@"Identyfikator płatności: <b>{0}</b><br>", s);
        }

        private void Ensure(ref BLL.Models.Klient iok)
        {
            if (iok == null) iok = new BLL.Models.Klient(item.Web, BLL.Tools.Get_LookupId(item, "selKlient"));
        }

        private void WriteToHistoryLog(string description, string outcome)
        {
            SPWeb web = workflowProperties.Web;
            Guid workflow = workflowProperties.WorkflowId;

            TimeSpan ts = new TimeSpan();
            SPWorkflow.CreateHistoryEvent(web, workflow, 0, web.CurrentUser, ts,
                outcome, description, string.Empty);
        }
        #endregion

        private void onWorkflowActivated1_Invoked(object sender, ExternalDataEventArgs e)
        {
            Debug.WriteLine("wfPrzygotujWiadomoscDlaKlienta - ACTIVATED");
            item = workflowProperties.Item;
        }

        private void Manage_ZUS_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colZUS_StatusZadania", _ZADANIE_ZWOLNIONE))
            {
                sbZUS = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, _ZUS_HTML_TEMPLATE_NAME));
            }
        }

        private void Manage_ZUSPD_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colZUS_StatusZadania", _ZADANIE_ZWOLNIONE)
                && (BLL.Tools.Get_Flag(item, "colZUS_PIT-4R_Zalaczony")
                    | BLL.Tools.Get_Flag(item, "colZUS_PIT-8AR_Zalaczony")))
            {
                sbZUSPD = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, _ZUSPD_HTML_TEMPLATE_NAME));
            }
        }

        private void Manage_PD_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colPD_StatusZadania", _ZADANIE_ZWOLNIONE))
            {
                sbPD = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, _PD_HTML_TEMPLATE_NAME));
            }
        }

        private void Manage_VAT_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colVAT_StatusZadania", _ZADANIE_ZWOLNIONE))
            {
                sbVAT = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, _VAT_HTML_TEMPLATE_NAME));
            }
        }

        private void Manage_RBR_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colRBR_StatusZadania", _ZADANIE_ZWOLNIONE))
            {
                sbRBR = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, _RBR_HTML_TEMPLATE_NAME));
            }
        }

        private void hasDaneDoWysylki(object sender, ConditionalEventArgs e)
        {
            if (sbZUS != null | sbZUSPD != null | sbPD != null | sbVAT != null | sbRBR != null)
                e.Result = true;
        }

        private void Create_ZUS_ExecuteCode(object sender, EventArgs e)
        {
            if (sbZUS != null & HasStatus(item, "colZUS_StatusWysylki", string.Empty))
            {
                //z karty kontrolnej
                ReplaceCurrency(sbZUS, item, "colZUS_SP_Skladka");
                ReplaceCurrency(sbZUS, item, "colZUS_ZD_Skladka");
                ReplaceCurrency(sbZUS, item, "colZUS_FP_Skladka");
                ReplaceDate(sbZUS, item, "colZUS_TerminPlatnosciSkladek");

                //z parametrów klienta
                ReplaceString(sbZUS, "colZUS_SP_Konto", BLL.Tools.Format_Konto(BLL.admSetup.GetValue(item.Web, "ZUS_SP_KONTO")));
                ReplaceString(sbZUS, "colZUS_ZD_Konto", BLL.Tools.Format_Konto(BLL.admSetup.GetValue(item.Web, "ZUS_ZD_KONTO")));
                ReplaceString(sbZUS, "colZUS_FP_Konto", BLL.Tools.Format_Konto(BLL.admSetup.GetValue(item.Web, "ZUS_FP_KONTO")));

                //wyliczane dynamicznie
                sbZUS.Replace("[[Tytul]]", _ZUS_TYTUL);

                StringBuilder sbInfo = new StringBuilder();

                if (BLL.Tools.Get_Value(item, "colZUS_SP_Skladka") > 0
                    | BLL.Tools.Get_Value(item, "colZUS_ZD_Skladka") > 0
                    | BLL.Tools.Get_Value(item, "colZUS_FP_Skladka") > 0)
                {
                    string ip = Format_IdPlat(BLL.tabOkresy.Get_IdPlatZUS(item.Web, BLL.Tools.Get_LookupId(item, "selOkres")));
                    sbInfo.Append(ip.ToString());
                }

                string taskInfo = BLL.tabZadania.Get_InfoDlaKlientaById(item.Web, BLL.Tools.Get_LookupId(item, "selZadanie_ZUS"));
                if (!string.IsNullOrEmpty(taskInfo)) sbInfo.Append(taskInfo);

                ReplaceString(sbZUS, "colInformacjaDlaKlienta", sbInfo.ToString());
            }
            else
            {
                sbZUS = null;
            }
        }



        private void Create_ZUSPD_ExecuteCode(object sender, EventArgs e)
        {
            if (sbZUSPD != null & HasStatus(item, "colZUS_StatusWysylki", string.Empty))
            {
                //z karty kontrolnej
                ReplaceCurrency(sbZUSPD, item, "colZUS_PIT-4R");
                ReplaceCurrency(sbZUSPD, item, "colZUS_PIT-8AR");
                ReplaceDate(sbZUSPD,
                    item,
                    "colZUSPD_TerminPlatnosciPodatku",
                    BLL.tabOkresy.Get_TerminPlatnosciByOkresId(
                        item.Web,
                        "colPD_TerminPlatnosciPodatku",
                        BLL.Tools.Get_LookupId(item, "selOkres")));

                //z parametrów klienta
                Ensure(ref iok);
                sbZUSPD.Replace("___colZUSPD_Konto___", BLL.Tools.Format_Konto(iok.NumerRachunkuZUSPD));

                //wyliczane dynamicznie
                sbZUSPD.Replace("[[Tytul]]", _ZUSPD_TYTUL);

                StringBuilder sbInfo = new StringBuilder();

                if (BLL.Tools.Get_Value(item, "colZUS_PIT-4R") > 0
                    | BLL.Tools.Get_Value(item, "colZUS_PIT-8AR") > 0)
                {
                    string ip = Format_IdPlat(BLL.tabOkresy.Get_IdPlatZUS(item.Web, BLL.Tools.Get_LookupId(item, "selOkres")));
                    sbInfo.Append(ip);
                }

                string taskInfo = BLL.tabZadania.Get_InfoDlaKlienta2ById(item.Web, BLL.Tools.Get_LookupId(item, "selZadanie_ZUS"));
                if (!string.IsNullOrEmpty(taskInfo)) sbInfo.Append(taskInfo);

                ReplaceString(sbZUSPD, "colInformacjaDlaKlienta", sbInfo.ToString());
            }
            else
            {
                sbZUSPD = null;
            }
        }

        private void Create_PD_ExecuteCode(object sender, EventArgs e)
        {
            if (sbPD != null & HasStatus(item, "colPD_StatusWysylki", string.Empty))
            {
                //z kartotek
                ReplaceCurrency(sbPD, item, "colPD_WartoscDoZaplaty");
                ReplaceDate(sbPD, item, "colPD_TerminPlatnosciPodatku");

                //z parametrów klienta
                Ensure(ref iok);
                ReplaceString(sbPD, "colPD_Konto", BLL.Tools.Format_Konto(iok.NumerRachunkuPD));

                //wyliczane dynamicznie
                string opcja = BLL.Tools.Get_Text(item, "colPD_OcenaWyniku");
                switch (opcja)
                {
                    case "Dochód":
                        sbPD.Replace("[[Tytul]]", _PD_TYTUL);
                        sbPD.Replace("[[OpisOcenyWyniku]]", "Wartość dochodu");
                        sbPD.Replace("[[colPD_Wartosc]]", BLL.Tools.Format_Currency(BLL.Tools.Get_Value(item, "colPD_WartoscDochodu")));

                        break;
                    case "Strata":
                        sbPD.Replace("[[Tytul]]", _PD_TYTUL_STRATA);
                        sbPD.Replace("[[OpisOcenyWyniku]]", "Wartość straty");
                        sbPD.Replace("[[colPD_Wartosc]]", BLL.Tools.Format_Currency(BLL.Tools.Get_Value(item, "colPD_WartoscStraty")));
                        break;
                    default:
                        sbPD.Replace("[[Tytul]]", string.Empty);
                        sbPD.Replace("[[OpisOcenyWyniku]]", string.Empty);
                        sbPD.Replace("[[colPD_Wartosc]]", string.Empty);
                        break;
                }

                string fop = BLL.Tools.Get_Text(item, "colFormaOpodatkowaniaPD");
                switch (fop)
                {
                    case "Zasady ogólne":
                        ReplaceString(sbPD, "colFormaOpodatkowaniaPD", "PIT-5");
                        break;
                    case "Podatek liniowy":
                        ReplaceString(sbPD, "colFormaOpodatkowaniaPD", "PIT-5L");
                        break;
                    case "Karta podatkowa":
                        ReplaceString(sbPD, "colFormaOpodatkowaniaPD", "PPE");
                        break;
                    case "Ryczałt":
                        ReplaceString(sbPD, "colFormaOpodatkowaniaPD", "CIT-8");
                        break;
                    default:
                        ReplaceString(sbPD, "colFormaOpodatkowaniaPD", string.Empty);
                        break;
                }

                string ip = string.Empty;
                string cykl = BLL.Tools.Get_Text(item, "enumRozliczeniePD");

                switch (cykl)
                {
                    case "Miesięcznie":
                        ip = BLL.tabOkresy.Get_IdPlat_Miesiecznie(item.Web, BLL.Tools.Get_LookupId(item, "selOkres"));
                        break;

                    case "Kwartalnie":
                        ip = BLL.tabOkresy.Get_IdPlat_Kwartalnie(item.Web, BLL.Tools.Get_LookupId(item, "selOkres"));
                        break;

                    default:
                        //nie dotyczy
                        break;
                }

                StringBuilder sbInfo = new StringBuilder();

                if (BLL.Tools.Get_Value(item, "colPD_WartoscDoZaplaty") > 0)
                {
                    ip = Format_IdPlat(ip);
                    sbInfo.Append(ip);
                }

                string taskInfo = BLL.tabZadania.Get_InfoDlaKlientaById(item.Web, BLL.Tools.Get_LookupId(item, "selZadanie_PD"));
                if (!string.IsNullOrEmpty(taskInfo)) sbInfo.Append(taskInfo);

                ReplaceString(sbPD, "colInformacjaDlaKlienta", sbInfo.ToString());

            }
            else
            {
                sbPD = null;
            }
        }

        private void Create_VAT_ExecuteCode(object sender, EventArgs e)
        {
            if (sbVAT != null & HasStatus(item, "colVAT_StatusWysylki", string.Empty))
            {
                //z karty kontrolnej
                ReplaceCurrency(sbVAT, item, "colVAT_WartoscNadwyzkiZaPoprzedniMiesiac");
                ReplaceCurrency(sbVAT, item, "colVAT_WartoscDoZaplaty");
                //ReplaceCurrency(sbVAT, item, "colVAT_WartoscDoPrzeniesienia");
                //ReplaceCurrency(sbVAT, item, "colVAT_WartoscDoZwrotu");
                ReplaceDate(sbVAT, item, "colVAT_TerminPlatnosciPodatku");
                ReplaceString(sbVAT, item, "colFormaOpodatkowaniaVAT");
                //ReplaceString(sbVAT, item, "colVAT_TerminZwrotuPodatku");
                ReplaceString(sbVAT, item, "colVAT_Decyzja");

                //z parametrów klienta
                Ensure(ref iok);
                ReplaceString(sbVAT, "colVAT_Konto", BLL.Tools.Format_Konto(iok.NumerRachunkuVAT));


                string szablon;

                //wyliczane dynamicznie
                string opcja = BLL.Tools.Get_Text(item, "colVAT_Decyzja");
                switch (opcja)
                {
                    case "Do zapłaty":
                        sbVAT.Replace("[[Tytul]]", _VAT_TYTUL);

                        sbVAT.Replace("[[VAT.TR]]", string.Empty);
                        break;
                    case "Do przeniesienia":
                        sbVAT.Replace("[[Tytul]]", _VAT_TYTUL_PRZENIESIENIE);
                        Append_VAT_DoPrzeniesienia();

                        break;
                    case "Do zwrotu":
                        sbVAT.Replace("[[Tytul]]", _VAT_TYTUL_ZWROT);

                        Append_VAT_DoZwrotu();

                        break;
                    case "Do przeniesienia i do zwrotu":
                        sbVAT.Replace("[[Tytul]]", _VAT_TYTUL_PRZENIESIENIE_ZWROT);

                        Append_VAT_DoPrzeniesienia();
                        Append_VAT_DoZwrotu();

                        break;
                    default:
                        sbVAT.Replace("[[Tytul]]", string.Empty);
                        break;
                }

                string ip = string.Empty;
                string cykl = BLL.Tools.Get_Text(item, "enumRozliczenieVAT");

                switch (cykl)
                {
                    case "Miesięcznie":
                        ip = BLL.tabOkresy.Get_IdPlat_Miesiecznie(item.Web, BLL.Tools.Get_LookupId(item, "selOkres"));
                        break;

                    case "Kwartalnie":
                        ip = BLL.tabOkresy.Get_IdPlat_Kwartalnie(item.Web, BLL.Tools.Get_LookupId(item, "selOkres"));
                        break;

                    default:
                        //nie dotyczy
                        break;
                }

                StringBuilder sbInfo = new StringBuilder();
                if (BLL.Tools.Get_Value(item, "colVAT_WartoscDoZaplaty") > 0)
                {
                    ip = Format_IdPlat(ip);
                    sbInfo.Append(ip);
                }

                string taskInfo = BLL.tabZadania.Get_InfoDlaKlientaById(item.Web, BLL.Tools.Get_LookupId(item, "selZadanie_VAT"));
                if (!string.IsNullOrEmpty(taskInfo)) sbInfo.Append(taskInfo);

                ReplaceString(sbVAT, "colInformacjaDlaKlienta", sbInfo.ToString());
            }
            else
            {
                sbVAT = null;
            }

        }

        private void Append_VAT_DoZwrotu()
        {
            string szablon;
            //do zwrotu
            szablon = BLL.dicSzablonyKomunikacji.Get_HTMLByKod(item.Web, "VAT.TR_TEMPLATE.Include");
            szablon = szablon.Replace("[[Opis]]", string.Format("Wartość do zwrtoru ({0})", BLL.Tools.Get_Text(item, "colVAT_TerminZwrotuPodatku")));
            szablon = szablon.Replace("[[Wartosc]]", BLL.Tools.Format_Currency(BLL.Tools.Get_Value(item, "colVAT_WartoscDoPrzeniesienia")));

            sbVAT.Replace("[[VAT.TR]]", szablon);
        }

        private void Append_VAT_DoPrzeniesienia()
        {
            string szablon;
            //do przeniesienia
            szablon = BLL.dicSzablonyKomunikacji.Get_HTMLByKod(item.Web, "VAT.TR_TEMPLATE.Include");
            szablon = szablon.Replace("[[Opis]]", "Wartość do przeniesienia");
            szablon = szablon.Replace("[[Wartosc]]", BLL.Tools.Format_Currency(BLL.Tools.Get_Value(item, "colVAT_WartoscDoPrzeniesienia")));

            sbVAT.Replace("[[VAT.TR]]", szablon);
        }

        private void Create_RBR_ExecuteCode(object sender, EventArgs e)
        {
            if (sbRBR != null & HasStatus(item, "colRBR_StatusWysylki", string.Empty))
            {
                ReplaceDate(sbRBR, item, "colBR_DataWystawieniaFaktury");
                ReplaceDate(sbRBR, item, "colBR_TerminPlatnosci");
                ReplaceString(sbRBR, item, "colBR_NumerFaktury");
                ReplaceCurrency(sbRBR, item, "colBR_WartoscDoZaplaty");

                //z parametrów systemu
                ReplaceString(sbRBR, "colBR_Konto", BLL.Tools.Format_Konto(BLL.admSetup.GetValue(item.Web, "BR_KONTO")));

                //wyliczane dynamicznie
                sbRBR.Replace("[[Tytul]]", _RBR_TYTUL);

                StringBuilder sbInfo = new StringBuilder();
                string taskInfo = BLL.tabZadania.Get_InfoDlaKlientaById(item.Web, BLL.Tools.Get_LookupId(item, "selZadanie_RBR"));
                if (!string.IsNullOrEmpty(taskInfo)) sbInfo.Append(taskInfo);

                ReplaceString(sbRBR, "colInformacjaDlaKlienta", sbInfo.ToString());
            }
            else
            {
                sbRBR = null;
            }
        }

        private void Create_Footer_ExecuteCode(object sender, EventArgs e)
        {
            sbFooter = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, _FOOTER_HTML_TEMPLATE_NAME));

            //określ osobę w stopce wiadomości
            int operatorId = Get_OperatorDoPodpisuWiadomosci();

            if (operatorId > 0)
            {
                string imieNazwisko, email, telefon;

                BLL.dicOperatorzy.Get_OperatorDetailsById(item.Web, operatorId, out imieNazwisko, out email, out telefon);

                //z parametrów operatora
                ReplaceString(sbFooter, "ImieNazwisko", imieNazwisko);
                ReplaceString(sbFooter, "colEmail", email);
                ReplaceString(sbFooter, "colTelefon", telefon);
            }
            else
            {
                ReplaceString(sbFooter, "ImieNazwisko", BLL.admSetup.GetValue(item.Web, "NAZWA_OPERATORA"));
                ReplaceString(sbFooter, "colEmail", BLL.admSetup.GetValue(item.Web, "EMAIL_BIURA"));
                ReplaceString(sbFooter, "colTelefon", BLL.admSetup.GetValue(item.Web, "TELEFON_BIURA"));
            }

            selectedOperatorId = operatorId;

        }

        private int Get_OperatorDoPodpisuWiadomosci()
        {
            int operatorId = 0;

            //najważniejszy jest operator obługujący podatki
            if (sbPD != null) operatorId = BLL.Tools.Get_LookupId(item, "selOperator_PD");
            if (operatorId > 0) return operatorId;

            //sprawdź ZUS
            if (sbZUS != null) operatorId = BLL.Tools.Get_LookupId(item, "selOperator_ZUS");
            if (operatorId > 0) return operatorId;

            //sprawdź ZUSPD
            if (sbZUSPD != null) operatorId = BLL.Tools.Get_LookupId(item, "selOperator_ZUS");
            if (operatorId > 0) return operatorId;

            //sprawdź VAT
            if (sbVAT != null) operatorId = BLL.Tools.Get_LookupId(item, "selOperator_VAT");
            if (operatorId > 0) return operatorId;

            //sprawdź RBR
            if (sbRBR != null) operatorId = BLL.Tools.Get_LookupId(item, "selOperator_RBR");
            if (operatorId > 0) return operatorId;

            return operatorId;
        }

        private void Create_Body_ExecuteCode(object sender, EventArgs e)
        {
            //footer
            string footerPlaceholder = "[[FOOTER.TR]]";

            sbBody = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, _BODY_HTML_TEMPLATE_NAME));
            if (sbFooter != null)
            {
                sbBody.Replace(footerPlaceholder, sbFooter.ToString());
            }
            else
            {
                //usuń stopkę z wiadomości
                sbBody.Replace(footerPlaceholder, string.Empty);
            }

            //body

            StringBuilder sb = new StringBuilder();
            int sCounter = 0;
            if (sbZUS != null)
            {
                AppendSeparator(sb, sCounter);
                sb.Append(sbZUS.ToString());
                sCounter++;

            }
            if (sbZUSPD != null)
            {
                AppendSeparator(sb, sCounter);
                sb.Append(sbZUSPD.ToString());
                sCounter++;
            }
            if (sbPD != null)
            {
                AppendSeparator(sb, sCounter);
                sb.Append(sbPD.ToString());
                sCounter++;
            }
            if (sbVAT != null)
            {
                AppendSeparator(sb, sCounter);
                sb.Append(sbVAT.ToString());
                sCounter++;
            }
            if (sbRBR != null)
            {
                AppendSeparator(sb, sCounter);
                sb.Append(sbRBR.ToString());
                sCounter++;
            }


            sbBody.Replace("[[BODY]]", sb.ToString());
        }

        private void Create_Message_ExecuteCode(object sender, EventArgs e)
        {
            int klientId = BLL.Tools.Get_LookupId(item, "selKlient");

            string nadawca = mailFrom;
            string odbiorca = mailTo;

            string kopiaDla = BLL.Tools.Append_EmailCC(item.Web, klientId, string.Empty);

            string temat = mailSubject;
            string trescHTML = sbBody.ToString();


            BLL.tabWiadomosci.Ensure_ColumnExist(item.Web, "_KartaKontrolnaId");
            int messageId = BLL.tabWiadomosci.AddNew(item.Web, item, nadawca, odbiorca, kopiaDla, false, false, temat, string.Empty, trescHTML, new DateTime(), 0, klientId, item.ID, BLL.Models.Marker.Ignore);

            ArrayList komponenty = Get_Komponenty();
            BLL.tabWiadomosci.Update_Komponenty(item.Web, messageId, komponenty);

            if (messageId > 0) messageCreated = true;
            else messageCreated = false;

        }

        private ArrayList Get_Komponenty()
        {
            ArrayList a = new ArrayList();
            if (sbZUS != null) a.Add("ZUS");
            if (sbZUSPD != null) a.Add("ZUSPD");
            if (sbPD != null) a.Add("PD");
            if (sbVAT != null) a.Add("VAT");
            if (sbRBR != null) a.Add("RBR");

            return a;

        }

        private void isMessage_Created(object sender, ConditionalEventArgs e)
        {
            if (messageCreated) e.Result = true;
            else e.Result = false;
        }

        private void Update_StatusyKK_ExecuteCode(object sender, EventArgs e)
        {
            if (sbZUS != null)
            {
                Update_StatusZadania(item, BLL.Tools.Get_LookupId(item, "selZadanie_ZUS"), "colZUS_StatusZadania", _STATUS_ZADANIA_WYSYLKA);
                Set_StatusWysylki(item, "colZUS_StatusWysylki", "Oczekuje");
            }
            if (sbZUSPD != null)
            {
                Update_StatusZadania(item, BLL.Tools.Get_LookupId(item, "selZadanie_ZUS"), "colZUS_StatusZadania", _STATUS_ZADANIA_WYSYLKA);
                Set_StatusWysylki(item, "colZUSPD_StatusWysylki", "Oczekuje");
            }
            if (sbPD != null)
            {
                Update_StatusZadania(item, BLL.Tools.Get_LookupId(item, "selZadanie_PD"), "colPD_StatusZadania", _STATUS_ZADANIA_WYSYLKA);
                Set_StatusWysylki(item, "colPD_StatusWysylki", "Oczekuje");
            }
            if (sbVAT != null)
            {
                Update_StatusZadania(item, BLL.Tools.Get_LookupId(item, "selZadanie_VAT"), "colVAT_StatusZadania", _STATUS_ZADANIA_WYSYLKA);
                Set_StatusWysylki(item, "colVAT_StatusWysylki", "Oczekuje");
            }
            if (sbRBR != null)
            {
                Update_StatusZadania(item, BLL.Tools.Get_LookupId(item, "selZadanie_RBR"), "colRBR_StatusZadania", _STATUS_ZADANIA_WYSYLKA);
                Set_StatusWysylki(item, "colRBR_StatusWysylki", "Oczekuje");
            }

            item.SystemUpdate();
        }

        private void Update_StatusyZadanpowiazanych_ExecuteCode(object sender, EventArgs e)
        {
            if (sbZUS != null)
            {
                int zadanieId = BLL.Tools.Get_LookupId(item, "selZadanie_ZUS");
                Update_RelatedTaskStatus(item, zadanieId, _STATUS_ZADANIA_WYSYLKA);

            }
            if (sbZUSPD != null)
            {
                int zadanieId = BLL.Tools.Get_LookupId(item, "selZadanie_ZUS");
                if (zadanieId > 0) Update_RelatedTaskStatus(item, zadanieId, _STATUS_ZADANIA_WYSYLKA);
            }
            if (sbPD != null)
            {
                int zadanieId = BLL.Tools.Get_LookupId(item, "selZadanie_PD");
                if (zadanieId > 0) Update_RelatedTaskStatus(item, zadanieId, _STATUS_ZADANIA_WYSYLKA);
            }
            if (sbVAT != null)
            {
                int zadanieId = BLL.Tools.Get_LookupId(item, "selZadanie_VAT");
                if (zadanieId > 0) Update_RelatedTaskStatus(item, zadanieId, _STATUS_ZADANIA_WYSYLKA);
            }
            if (sbRBR != null)
            {
                int zadanieId = BLL.Tools.Get_LookupId(item, "selZadanie_RBR");
                if (zadanieId > 0) Update_RelatedTaskStatus(item, zadanieId, _STATUS_ZADANIA_WYSYLKA);
            }
        }

        private void Update_StatusZadania(SPListItem item, int zadanieId, string col, string value)
        {
            Debug.WriteLine("Workflows.PrzygotujWiadomosc.Update_StatusZadania");
            Debug.WriteLine("kkId=" + item.ID.ToString());
            Debug.WriteLine("zadanieId=" + zadanieId.ToString());

            BLL.Tools.Set_Text(item, col, value);
            Debug.WriteLine("statusZadania=" + value);

        }

        private void Update_RelatedTaskStatus(SPListItem item, int zadanieId, string value)
        {
            //aktulizuj status skojarzonego zadania
            if (zadanieId > 0)
            {
                BLL.tabZadania.Update_StatusZadania(item.Web, zadanieId, value);
                WriteToHistoryLog(
                    string.Format("Zadanie #{0} zaktualizowane", zadanieId.ToString()),
                    "Status=" + value);
            }
        }

        private void Set_StatusWysylki(SPListItem item, string col, string s)
        {
            Debug.WriteLine("Workflows.PrzygotujWiadomosc.Update_StatusWysylki");
            Debug.WriteLine("kkId=" + item.ID.ToString());

            BLL.Tools.Set_Text(item, col, s);
            Debug.WriteLine("statusWysylki=" + s);
        }


        private void Create_Subject_ExecuteCode(object sender, EventArgs e)
        {
            Ensure(ref iok);
            mailSubject = string.Format(":: Wyniki finansowe na koniec {0} [{1}] dla {2}",
                BLL.Tools.Get_LookupValue(item, "selOkres"),
                Get_UwzglednioneSkladniki(),
                iok.NazwaPrezentowana);
        }

        private string Get_UwzglednioneSkladniki()
        {
            string result = string.Empty;
            if (sbZUS != null) result = result + "ZUS";
            if (sbZUSPD != null) result = result + ",PD.Prac";
            if (sbPD != null) result = result + ",PD";
            if (sbVAT != null) result = result + ",VAT";
            if (sbRBR != null) result = result + ",Rozliczenie";

            if (result.StartsWith(",")) result = result.Substring(1, result.Length - 1);
            return result;

        }

        private void Create_Odbiorcy_ExecuteCode(object sender, EventArgs e)
        {
            Ensure(ref iok);
            mailTo = iok.Email;
            mailFrom = BLL.admSetup.GetValue(item.Web, "EMAIL_DEFAULT_SENDER");
            if (selectedOperatorId > 0)
            {
                mailCC = BLL.dicOperatorzy.Get_EmailById(item.Web, selectedOperatorId);
            }
        }

        private void isZgodaNaWysylkeMaila(object sender, ConditionalEventArgs e)
        {
            Ensure(ref iok);
            if (iok.PreferowanaFormaKontaktu.Equals("Email"))
            {
                if (string.IsNullOrEmpty(iok.Email)) e.Result = false;
                else e.Result = true;
            }
            else e.Result = false;
        }




    }
}
