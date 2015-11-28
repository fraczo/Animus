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

namespace Workflows.WyslijDoKlienta
{
    public sealed partial class WyslijDoKlienta : SequentialWorkflowActivity
    {
        public WyslijDoKlienta()
        {
            InitializeComponent();
        }

        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();
        private SPListItem item;

        private StringBuilder sbZUS;
        private StringBuilder sbZUSPD;
        private StringBuilder sbPD;
        private StringBuilder sbVAT;
        private StringBuilder sbRBR;

        private const string _ZADANIE_ZWOLNIONE = "Zwolnione do wysyłki";

        private const string _ZUS_HTML_TEMPLATE_NAME = "ZUS_TEMPLATE";
        private const string _ZUSPD_HTML_TEMPLATE_NAME = "ZUSPD_TEMPLATE";
        private const string _PD_HTML_TEMPLATE_NAME = "PD_TEMPLATE";
        private const string _VAT_HTML_TEMPLATE_NAME = "VAT_TEMPLATE";
        private const string _RBR_HTML_TEMPLATE_NAME = "RBR_TEMPLATE";

        private void onWorkflowActivated1_Invoked(object sender, ExternalDataEventArgs e)
        {
            item = workflowProperties.Item;
        }

        #region Helpers
        private bool HasStatus(SPListItem item, string col, string status)
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

        private void ReplaceString(StringBuilder sb, SPListItem item, string col, string s)
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
        #endregion

        private void Manage_ZUS_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colZUS_StatusZadania", _ZADANIE_ZWOLNIONE))
            {
                sbZUS = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, _ZUS_HTML_TEMPLATE_NAME));
            }
        }

        private void Manage_ZUSPD_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colZUS_StatusZadania", _ZADANIE_ZWOLNIONE))
            {
                sbZUSPD = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, _ZUSPD_HTML_TEMPLATE_NAME));
            }
        }

        private void Manage_PD_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colZUS_StatusZadania", _ZADANIE_ZWOLNIONE))
            {
                sbPD = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, _PD_HTML_TEMPLATE_NAME));
            }
        }

        private void Manage_VAT_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colZUS_StatusZadania", _ZADANIE_ZWOLNIONE))
            {
                sbZUS = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, _VAT_HTML_TEMPLATE_NAME));
            }
        }

        private void Manage_RBR_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colZUS_StatusZadania", _ZADANIE_ZWOLNIONE))
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
            if (HasStatus(item, "colStatusWysylki", string.Empty))
            {
                //z karty kontrolnej
                ReplaceCurrency(sbZUS, item, "colZUS_SP_Skladka");
                ReplaceCurrency(sbZUS, item, "colZUS_ZD_Skladka");
                ReplaceCurrency(sbZUS, item, "colZUS_FP_Skladka");
                ReplaceDate(sbZUS, item, "colZUS_TerminPlatnosciSkladek");

                //z parametrów klienta
                ReplaceString(sbZUS, item, "colZUS_SP_Konto", "numer konta SP");
            }
            else
            {
                sbZUS = null;
            }
        }

        private void Create_ZUSPD_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colStatusWysylki", string.Empty))
            {
                //z karty kontrolnej
                ReplaceCurrency(sbZUSPD, item, "colZUS_PIT-4R");
                ReplaceCurrency(sbZUSPD, item, "colZUS_PIT-8AR");

                //z parametrów klienta
                ReplaceDate(sbZUSPD, item, "colPD_TerminPlatnosciSkladek"); //może nie być dotępny
                ReplaceString(sbZUSPD, item, "colPD_Konto", "numer konta dla podatku dochodowego");
            }
            else
            {
                sbZUSPD = null;
            }
        }

        private void Create_PD_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colStatusWysylki", string.Empty))
            {
                //z kartotek
                ReplaceCurrency(sbPD, item, "colPD_WartoscDochodu");
                ReplaceCurrency(sbPD, item, "colPD_WartoscStraty");
                ReplaceCurrency(sbPD, item, "colPD_WartoscDoZaplaty");

                //z parametrów klienta
                ReplaceDate(sbPD, item, "colPD_TerminPlatnosci"); //może nie być dotępny
                ReplaceString(sbPD, item, "colPD_Konto", "numer konta dla podatku dochodowego");
            }
            else
            {
                sbPD = null;
            }
        }

        private void Create_VAT_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colStatusWysylki", string.Empty))
            {
                //z karty kontrolnej
                ReplaceCurrency(sbVAT, item, "colVAT_WartoscNadwyzkiZaPoprzedniMiesiac");
                ReplaceCurrency(sbVAT, item, "colVAT_WartoscDoZaplaty");
                ReplaceCurrency(sbVAT, item, "colVAT_WartoscDoPrzeniesienia");
                ReplaceCurrency(sbVAT, item, "ccolVAT_WartoscDoZwrotu");
                ReplaceString(sbVAT, item, "colVAT_Decyzja");

                //z parametrów klienta
                ReplaceString(sbVAT, item, "colPD_Konto", "numer konta dla podatku dochodowego");
            }
            else
            {
                sbVAT = null;
            }
        }

        private void Create_RBR_ExecuteCode(object sender, EventArgs e)
        {
            if (HasStatus(item, "colStatusWysylki", string.Empty))
            {
                ReplaceDate(sbRBR, item, "colBR_DataWystawieniaFaktury");
                ReplaceDate(sbRBR, item, "colBR_TerminPlatnosci");
                ReplaceString(sbRBR, item, "colBR_NumerFaktury");
                ReplaceCurrency(sbRBR, item, "colBR_WartoscDoZaplaty");
            }
            else
            {
                sbRBR = null;
            }
        }

        private void Create_Body_ExecuteCode(object sender, EventArgs e)
        {

        }

        private void Create_Footer_ExecuteCode(object sender, EventArgs e)
        {

        }

        private void Create_Message_ExecuteCode(object sender, EventArgs e)
        {

        }
    }
}