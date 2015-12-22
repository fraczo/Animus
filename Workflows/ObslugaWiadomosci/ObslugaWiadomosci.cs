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
using System.Net.Mail;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Diagnostics;

namespace Workflows.ObslugaWiadomosci
{
    public sealed partial class ObslugaWiadomosci : SequentialWorkflowActivity
    {
        public ObslugaWiadomosci()
        {
            InitializeComponent();
        }

        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();
        public SPListItem item;
        public MailMessage mail;
        private bool isMailReadyToSend;
        private string _STATUS_WYSYLKI_WYSLANA = "Wysłana";
        private string _STATUS_ZADANIE_ZAKONCZONE = "Zakończone";
        private DateTime dataWyslania;
        private bool isMessageSent = false;

        private void onWorkflowActivated1_Invoked(object sender, ExternalDataEventArgs e)
        {
            Debug.WriteLine("ObslugaWiadomosci - ACTIVATED");

            item = workflowProperties.Item;
        }

        private void Mail_Setup_ExecuteCode(object sender, EventArgs e)
        {
            mail = new MailMessage();
            isMailReadyToSend = false;

            //From
            if (item["colNadawca"] != null)
            {
                mail.From = new MailAddress(item["colNadawca"].ToString());
            }
            else
            {
                mail.From = new MailAddress(BLL.admSetup.GetValue(item.ParentList.ParentWeb, "EMAIL_BIURA"));
            }

            //To
            if (item["colOdbiorca"] != null && !string.IsNullOrEmpty(item.Title))
            {
                mail.To.Add(new MailAddress(item["colOdbiorca"].ToString()));

                //CC
                bool isKopiaDoNadawcy = item["colKopiaDoNadawcy"] != null ? (bool)item["colKopiaDoNadawcy"] : false;
                if (isKopiaDoNadawcy)
                {
                    mail.CC.Add(new MailAddress(item["colNadawca"].ToString()));
                }

                //BCC
                bool isKopiaDoBiura = item["colKopiaDoBiura"] != null ? (bool)item["colKopiaDoBiura"] : false;
                if (isKopiaDoBiura)
                {
                    mail.Bcc.Add(new MailAddress(BLL.admSetup.GetValue(item.ParentList.ParentWeb, "EMAIL_BIURA_ARCH")));
                }

                //Subject
                mail.Subject = item.Title;

                //Body
                if (item["colTrescHTML"] != null)
                {
                    string bodyHTML = item["colTrescHTML"].ToString();
                    mail.Body = bodyHTML;
                    mail.IsBodyHtml = true;
                }
                else
                {
                    if (item["colTresc"] != null)
                    {
                        string body = item["colTresc"].ToString();

                        StringBuilder sb = new StringBuilder(BLL.dicSzablonyKomunikacji.Get_TemplateByKod(item, "EMAIL_DEFAULT_BODY", true));
                        sb.Replace(@"___BODY___", body);
                        sb.Replace(@"___FOOTER___", string.Empty);
                        mail.Body = sb.ToString();
                        mail.IsBodyHtml = true;
                    }
                }

                isMailReadyToSend = true;
            }
        }

        private void Mail_Send_ExecuteCode(object sender, EventArgs e)
        {
            if (isMailReadyToSend)
            {
                bool testMode = true;

                try
                {
                    if (BLL.admSetup.IsProductionEnabled(item.Web))
                    {
                        //TRYB PRODUKCYNJY AKTYWNY
                        testMode = false;
                    }
                }
                catch (Exception ex)
                {
                    var r = ElasticEmail.EmailGenerator.ReportError(ex, item.ParentList.ParentWeb.Url);
                }


                bool result = SPEmail.EmailGenerator.SendMailFromMessageQueue(item, mail, testMode);

                if (result)
                {
                    isMessageSent = true;
                    //ustaw flagę wysyłki
                    item["colCzyWyslana"] = true;

                    dataWyslania = DateTime.Now;
                    item["colDataNadania"] = dataWyslania;
                    item.Update();
                }
                else
                {
                    var r = ElasticEmail.EmailGenerator.SendMail(string.Format(@"Animus Message#{0} not sent", item.ID.ToString()), string.Empty);
                }
            }
        }

        private void isWiadomoscWyslana(object sender, ConditionalEventArgs e)
        {
            e.Result = isMessageSent;
        }


        private void isFlagaWysłanoUstawiona(object sender, ConditionalEventArgs e)
        {
            e.Result = item["colCzyWyslana"] != null ? (bool)item["colCzyWyslana"] : false;
        }

        private void setState_Anulowana_MethodInvoking(object sender, EventArgs e)
        {
            //SetStatusWysylki(enumStatusWysylki.Anulowana);
        }

        private void setState_Wysłana_MethodInvoking(object sender, EventArgs e)
        {
            SetStatusWysylki(enumStatusWysylki.Wysłana);
        }

        private void setState_PrzygotowanieWysyłki_MethodInvoking(object sender, EventArgs e)
        {
            SetStatusWysylki(enumStatusWysylki.Obsługa);
        }

        private void SetStatusWysylki(enumStatusWysylki status)
        {
            item["enumStatusWysylki"] = status;
            item["colDataNadania"] = DateTime.Now;
            item.SystemUpdate();
        }

        private void ReportError_ExecuteCode(object sender, EventArgs e)
        {
            Workflows.Tools.ReportErrorToHistoryLog(workflowProperties, sender, true);
        }

        private void codeActivity1_ExecuteCode(object sender, EventArgs e)
        {
            //do testów reset flagi
            item["colCzyWyslana"] = 0;
            item.SystemUpdate();
        }

        public String logError_HistoryDescription = default(System.String);

        private void IsZadanieToBeUpdated(object sender, ConditionalEventArgs e)
        {
            e.Result = (BLL.Tools.Get_Value(item, "_ZadanieId") > 0);
        }

        private void isKartaKontrolnaToBeUpdated(object sender, ConditionalEventArgs e)
        {
            e.Result = (BLL.Tools.Get_Value(item, "_ZadanieId") <= 0)
                && (BLL.Tools.Get_Value(item, "_KartaKontrolnaId") > 0);
        }

        private void Update_Zadanie_ExecuteCode(object sender, EventArgs e)
        {
            //dedykowane do obsługi wiadomości, pozostałe zadania są aktualizowane przez karty kontrolne.

            int zadanieId = (int)BLL.Tools.Get_Value(item, "_ZadanieId");

            SPListItem task = BLL.tabZadania.Get_ZadanieById(item.Web, zadanieId);
            if (task != null)
            {
                BLL.tabZadania.Update_StatusWysylki(item.Web, item, zadanieId, BLL.Models.StatusZadania.Zakończone);
                Workflows.Tools.WriteToHistoryLog(workflowProperties, "Zadanie #" + task.ID.ToString(), "zaktualizowane");

            }
        }

        private void Update_KK_Zadania_ExecuteCode(object sender, EventArgs e)
        {
            int kartaKontrolnaId = (int)BLL.Tools.Get_Value(item, "_KartaKontrolnaId");

            SPFieldMultiChoiceValue fmcv = BLL.Tools.Get_MutichoiceValue(item, "_KomponentyKK");

            if (fmcv.Count > 0)
            {
                SPListItem kkItem = BLL.tabKartyKontrolne.GetItemById(item.Web, kartaKontrolnaId);

                if (kkItem != null)
                {
                    for (int i = 0; i < fmcv.Count; i++)
                    {
                        SPListItem zadanie;

                        switch (fmcv[i].ToString())
                        {
                            case "ZUS":
                                BLL.tabKartyKontrolne.Set_StatusZadania(kkItem, "colZUS_StatusZadania", _STATUS_ZADANIE_ZAKONCZONE);
                                BLL.tabKartyKontrolne.Set_StatusWysylki(kkItem, "colZUS_StatusWysylki", _STATUS_WYSYLKI_WYSLANA);
                                BLL.tabKartyKontrolne.Set_DataWyslania(kkItem, "colZUS_DataWyslaniaInformacji", dataWyslania);

                                zadanie = BLL.tabZadania.GetItemById(item.Web, BLL.Tools.Get_LookupId(kkItem, "selZadanie_ZUS"));
                                if (zadanie != null)
                                {
                                    BLL.Tools.Set_Text(zadanie, "enumStatusZadania", _STATUS_ZADANIE_ZAKONCZONE);
                                    BLL.Tools.Set_Date(zadanie, "colZUS_DataWyslaniaInformacji", dataWyslania);

                                    zadanie.SystemUpdate();

                                    Workflows.Tools.WriteToHistoryLog(workflowProperties, "Zadanie ZUS #" + zadanie.ID.ToString(), "zaktualizowane");
                                }
                                break;
                            case "ZUSPD":
                                //BLL.tabKartyKontrolne.Set_StatusZadania(kkItem, "colZUS_StatusZadania", _STATUS_ZADANIE_ZAKONCZONE);
                                BLL.tabKartyKontrolne.Set_StatusWysylki(kkItem, "colZUSPD_StatusWysylki", _STATUS_WYSYLKI_WYSLANA);
                                BLL.tabKartyKontrolne.Set_DataWyslania(kkItem, "colZUSPD_DataWyslaniaInformacji", dataWyslania);

                                //zadanie = BLL.tabZadania.GetItemById(item.Web, BLL.Tools.Get_LookupId(kkItem, "selZadanie_ZUS"));
                                //if (zadanie != null)
                                //{
                                //    BLL.Tools.Set_Text(zadanie, "enumStatusZadania", _STATUS_ZADANIE_ZAKONCZONE);
                                //    BLL.Tools.Set_Date(zadanie, "colZUSPD_DataWyslaniaInformacji", dataWyslania);

                                //    zadanie.SystemUpdate();
                                //}
                                break;
                            case "PD":
                                BLL.tabKartyKontrolne.Set_StatusZadania(kkItem, "colPD_StatusZadania", _STATUS_ZADANIE_ZAKONCZONE);
                                BLL.tabKartyKontrolne.Set_StatusWysylki(kkItem, "colPD_StatusWysylki", _STATUS_WYSYLKI_WYSLANA);
                                BLL.tabKartyKontrolne.Set_DataWyslania(kkItem, "colPD_DataWylaniaInformacji", dataWyslania); //uwaga błąd w pisowni

                                zadanie = BLL.tabZadania.GetItemById(item.Web, BLL.Tools.Get_LookupId(kkItem, "selZadanie_PD"));
                                if (zadanie != null)
                                {
                                    BLL.Tools.Set_Text(zadanie, "enumStatusZadania", _STATUS_ZADANIE_ZAKONCZONE);
                                    BLL.Tools.Set_Date(zadanie, "colPD_DataWylaniaInformacji", dataWyslania);

                                    zadanie.SystemUpdate();
                                    Workflows.Tools.WriteToHistoryLog(workflowProperties, "Zadanie PD #" + zadanie.ID.ToString(), "zaktualizowane");
                                }
                                break;
                            case "VAT":
                                BLL.tabKartyKontrolne.Set_StatusZadania(kkItem, "colVAT_StatusZadania", _STATUS_ZADANIE_ZAKONCZONE);
                                BLL.tabKartyKontrolne.Set_StatusWysylki(kkItem, "colVAT_StatusWysylki", _STATUS_WYSYLKI_WYSLANA);
                                BLL.tabKartyKontrolne.Set_DataWyslania(kkItem, "colVAT_DataWyslaniaInformacji", dataWyslania);

                                zadanie = BLL.tabZadania.GetItemById(item.Web, BLL.Tools.Get_LookupId(kkItem, "selZadanie_VAT"));
                                if (zadanie != null)
                                {
                                    BLL.Tools.Set_Text(zadanie, "enumStatusZadania", _STATUS_ZADANIE_ZAKONCZONE);
                                    BLL.Tools.Set_Date(zadanie, "colVAT_DataWyslaniaInformacji", dataWyslania);

                                    zadanie.SystemUpdate();
                                    Workflows.Tools.WriteToHistoryLog(workflowProperties, "Zadanie VAT #" + zadanie.ID.ToString(), "zaktualizowane");
                                }
                                break;
                            case "RBR":
                                BLL.tabKartyKontrolne.Set_StatusZadania(kkItem, "colRBR_StatusZadania", _STATUS_ZADANIE_ZAKONCZONE);
                                BLL.tabKartyKontrolne.Set_StatusWysylki(kkItem, "colRBR_StatusWysylki", _STATUS_WYSYLKI_WYSLANA);
                                BLL.tabKartyKontrolne.Set_DataWyslania(kkItem, "colBR_DataWyslaniaInformacji", dataWyslania);

                                zadanie = BLL.tabZadania.GetItemById(item.Web, BLL.Tools.Get_LookupId(kkItem, "selZadanie_RBR"));
                                if (zadanie != null)
                                {
                                    BLL.Tools.Set_Text(zadanie, "enumStatusZadania", _STATUS_ZADANIE_ZAKONCZONE);
                                    BLL.Tools.Set_Date(zadanie, "colBR_DataWyslaniaInformacji", dataWyslania);

                                    zadanie.SystemUpdate();
                                    Workflows.Tools.WriteToHistoryLog(workflowProperties, "Zadanie RBR #" + zadanie.ID.ToString(), "zaktualizowane");
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                kkItem.SystemUpdate();
                Workflows.Tools.WriteToHistoryLog(workflowProperties, "Karta Kontrolna #" + kkItem.ID.ToString(), "zaktualizowana");
            }

        }

    }
}
