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

namespace Workflows.GFR
{
    public sealed partial class GFR : SequentialWorkflowActivity
    {
        public GFR()
        {
            InitializeComponent();
        }

        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();
        SPListItem item;


        private void onWorkflowActivated1_Invoked(object sender, ExternalDataEventArgs e)
        {
            item = workflowProperties.Item;
        }

        private void isCT_GFR(object sender, ConditionalEventArgs e)
        {
            if (item.ContentType.Name.Equals("Generowanie formatek rozliczeniowych")) e.Result = true;
            else e.Result = false;

        }

        private void Select_Klienci_ExecuteCode(object sender, EventArgs e)
        {
            //wymaga elevated mode
            SPListItem item = workflowProperties.Item;
            EventReceivers.admProcesy.GFR_Request.Create(item);
        }

        private void UpdatStatusZadania_ExecuteCode(object sender, EventArgs e)
        {
            BLL.Tools.Set_Text(item, "enumStatusZlecenia", "Zakończone");
            item.SystemUpdate();
        }






    }
}
