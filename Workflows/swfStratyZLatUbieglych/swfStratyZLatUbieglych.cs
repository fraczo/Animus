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

namespace Workflows.swfStratyZLatUbieglych
{
    public sealed partial class swfStratyZLatUbieglych : SequentialWorkflowActivity
    {
        public swfStratyZLatUbieglych()
        {
            InitializeComponent();
        }

        public System.Collections.IEnumerator myEnum;
        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();
        public Array klienci;
        public Array rekordy;
        public SPList sList;


        int currentYear = DateTime.Now.Year;
        private int _YEAR_HISTORY_OFFSET = 5;

        private void Create_TargetList_ExecuteCode(object sender, EventArgs e)
        {
            Debug.WriteLine("Create_TargetList");

            klienci = BLL.tabKlienci.Get_AktywniKlienci(workflowProperties.Web);
            Debug.WriteLine("#klienci: " + klienci.Length.ToString());

            myEnum = klienci.GetEnumerator();
        }

        private void Get_ExistingKEYs_ExecuteCode(object sender, EventArgs e)
        {
            Debug.WriteLine("Get_ExistingKEYs");

            if (klienci.Length > 0)
            {
                int maxYear = currentYear - 1;
                int minYear = currentYear - 1 - _YEAR_HISTORY_OFFSET;
                Ensure(ref sList);
                rekordy = BLL.tabStratyZLatUbieglych.Get_ExistingRecords(sList, minYear, maxYear);
                Debug.WriteLine("#records: " + rekordy.Length.ToString());
            }
        }

        private void isCompleted(object sender, ConditionalEventArgs e)
        {
            if (myEnum.MoveNext() && myEnum != null) e.Result = true;
            else e.Result = false;
        }

        private void isRecordExist2(object sender, ConditionalEventArgs e)
        {
            e.Result = false;
        }

        private void Append_Records_ExecuteCode(object sender, EventArgs e)
        {
            SPListItem klient = (SPListItem)myEnum.Current;
            logNowyRekord_HistoryOutcome1 = string.Empty;

            Debug.WriteLine("Append_Records: klientId=" + klient.ID.ToString());

            int targetYear = currentYear;
            for (int i = _YEAR_HISTORY_OFFSET - 1; i >= 0; i--)
            {
                targetYear = currentYear - 1 - i;
                string key = BLL.tabStratyZLatUbieglych.Create_KEY(klient.ID, targetYear);

                if (NotUsed(key))
                {
                    int r = BLL.tabStratyZLatUbieglych.AddNew(sList, klient.ID, targetYear, key);
                    Debug.WriteLine("nowy rekord# " + r.ToString());
                }
            }
        }

        private void Ensure(ref SPList sList)
        {
            if (sList == null) sList = BLL.tabStratyZLatUbieglych.GetList(workflowProperties.Web);
        }

        /// <summary>
        /// sprawdza czy dany klucz nie został już użyty w innym rekordzie.
        /// </summary>
        private bool NotUsed(string key)
        {
            foreach (SPListItem item in rekordy)
            {
                if (BLL.Tools.Get_Text(item, "KEY").Equals(key))
                {
                    return false;
                }
            }

            return true;
        }

        private void sendInitiated_MethodInvoking(object sender, EventArgs e)
        {

        }

        private void onWorkflowActivated1_Invoked(object sender, ExternalDataEventArgs e)
        {
            Debug.WriteLine("swfStratyZLatUbieglych - ACTIVATED");
        }

        public static DependencyProperty logNowyRekord_HistoryOutcome1Property = DependencyProperty.Register("logNowyRekord_HistoryOutcome1", typeof(System.String), typeof(Workflows.swfStratyZLatUbieglych.swfStratyZLatUbieglych));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Misc")]
        public String logNowyRekord_HistoryOutcome1
        {
            get
            {
                return ((string)(base.GetValue(Workflows.swfStratyZLatUbieglych.swfStratyZLatUbieglych.logNowyRekord_HistoryOutcome1Property)));
            }
            set
            {
                base.SetValue(Workflows.swfStratyZLatUbieglych.swfStratyZLatUbieglych.logNowyRekord_HistoryOutcome1Property, value);
            }
        }






    }
}
