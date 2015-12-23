using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using BLL;

namespace EventReceivers.admProcesy
{
    public class admProcesy : SPItemEventReceiver
    {
        const string _CT_GFR_K = "Generowanie formatek rozliczeniowych dla klienta";
        const string _CT_GFR = "Generowanie formatek rozliczeniowych";
        private string _ZLECENIE_NOWY = "Nowy";
        private string _WF_GFR_K = "Generuj formatki rozliczeniowe dla klienta";
        private string _ZLECENIE_ZAKONCZONE = "Zakończony";
        private string _ZLECENIE_OBSLUGA = "Obsługa";
        private string _WF_OBSLUGA_KK = @"Obsługa kart kontrolnych";
        private string _WF_OBSLUGA_WIADOMOSCI_OCZEKUJACYCH = "Obsługa wiadomości oczekujących";

        public override void ItemAdded(SPItemEventProperties properties)
        {
            SPListItem item = properties.ListItem;
            BLL.Logger.LogEvent_EventReceiverInitiated(item);

            try
            {
                switch (item.ContentType.Name)
                {
                    case _CT_GFR_K:
                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            //GFR_K_Request.Create(item);
                            BLL.Logger.LogEvent_Procedure("WF:Generuj formatki rozliczeniowe dla klienta", item, "init");
                            BLL.Workflows.StartWorkflow(item, _WF_GFR_K);
                            BLL.Logger.LogEvent_Procedure("WF:Generuj formatki rozliczeniowe dla klienta", item, "end");
                        });
                        break;
                    case _CT_GFR:
                        //SPSecurity.RunWithElevatedPrivileges(delegate()
                        // {
                        this.EventFiringEnabled = false;
                        GFR_Request.Create(item);
                        this.EventFiringEnabled = true;

                        Start_GFR_K_Workflows(item);

                        //BLL.Logger.LogEvent_Procedure("WF:Generuj formatki rozliczeniowe", item, "init");
                        //BLL.Workflows.StartWorkflow(item, "Generuj formatki rozliczeniowe");
                        //});
                        break;
                    case "Obsługa wiadomości":
                        this.EventFiringEnabled = false;
                        BLL.Tools.Set_Text(item, "enumStatusZlecenia", _ZLECENIE_OBSLUGA);
                        item.SystemUpdate();

                        ObslugaWiadomosci.Execute(item);
                        //BLL.Workflows.StartSiteWorkflow(item.Web.Site, _WF_OBSLUGA_WIADOMOSCI_OCZEKUJACYCH); <<< używane przez timer job

                        BLL.Tools.Set_Text(item, "enumStatusZlecenia", _ZLECENIE_ZAKONCZONE);
                        item.SystemUpdate();
                        this.EventFiringEnabled = true;
                        break;
                    case "Przygotuj wiadomości z kart kontrolnych":
                        this.EventFiringEnabled = false;
                        BLL.Tools.Set_Text(item, "enumStatusZlecenia", _ZLECENIE_OBSLUGA);
                        item.SystemUpdate();
                        
                        ObslugaKartKontrolnych.Execute(item);
                        //BLL.Workflows.StartSiteWorkflow(item.Web.Site, _WF_OBSLUGA_KK); <<< używane przez timer job

                        BLL.Tools.Set_Text(item, "enumStatusZlecenia", _ZLECENIE_ZAKONCZONE);
                        item.SystemUpdate();
                        this.EventFiringEnabled = true;
                        break;
                    default:
                        break;
                }

                BLL.Logger.LogEvent_EventReceiverCompleted(item);
            }
            catch (Exception ex)
            {
                BLL.Logger.LogEvent("admProcsy.ItemAdded.error", ex.ToString());

                BLL.Tools.Set_Text(item, "_Memo", ex.ToString());
                BLL.Tools.Set_Text(item, "enumStatusZlecenia", "Anulowane");
                item.SystemUpdate();
            }
        }

        /// <summary>
        /// uruchamia workflow na zadaniach typu Generowanie formatek dla klienta w statusie Nowy
        /// </summary>
        private void Start_GFR_K_Workflows(SPListItem item)
        {
            Array zlecenia = BLL.admProcesy.SelectItems(item.Web, _CT_GFR_K, _ZLECENIE_NOWY, item.ID);
            foreach (SPListItem zlecenie in zlecenia)
            {
                BLL.Logger.LogEvent_Procedure("WF:Generuj formatki rozliczeniowe dla klienta", zlecenie, "init");
                BLL.Workflows.StartWorkflow(zlecenie, _WF_GFR_K);
                BLL.Logger.LogEvent_Procedure("WF:Generuj formatki rozliczeniowe dla klienta", zlecenie, "end");
            }

        }
    }
}
