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

namespace Workflows.GFR_K
{
    public sealed partial class GFR_K : SequentialWorkflowActivity
    {
        public GFR_K()
        {
            InitializeComponent();
        }

        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();
        private SPListItem item;

        private void Run_GFR_K_Create_ExecuteCode(object sender, EventArgs e)
        {
            //wymaga elevated mode
            SPListItem item = workflowProperties.Item;
            try
            {
                EventReceivers.admProcesy.GFR_K_Request.Create(item);
            }
            catch (Exception ex)
            {
                BLL.Logger.LogError("ERR: GFR_K_Create", ex.ToString(), ex);
            }
        }

        private void onWorkflowActivated1_Invoked(object sender, ExternalDataEventArgs e)
        {
            item = workflowProperties.Item;
        }

        private void isCT_GFRK(object sender, ConditionalEventArgs e)
        {
            if (item.ContentType.Name.Equals("Generowanie formatek rozliczeniowych dla klienta")) e.Result = true;
            else e.Result = false;
        }

        private void Status_Zakonczony_ExecuteCode(object sender, EventArgs e)
        {
            try
            {
                BLL.Tools.Set_Text(item, "enumStatusZlecenia", "Zakończony");
                item.SystemUpdate();
            }
            catch (Exception ex)
            {
                BLL.Logger.LogError("ERR: Update_StatusZlecenia", ex.ToString(), ex);
            }
        }

        private void Status_Obsluga_ExecuteCode(object sender, EventArgs e)
        {
            try
            {
                BLL.Tools.Set_Text(item, "enumStatusZlecenia", "Obsługa");
                item.SystemUpdate();
            }
            catch (Exception ex)
            {
                BLL.Logger.LogError("ERR: Update_StatusZlecenia", ex.ToString(), ex);
            }
        }

        private void Delete_Item_ExecuteCode(object sender, EventArgs e)
        {
            item.Delete();
        }



    }
}
