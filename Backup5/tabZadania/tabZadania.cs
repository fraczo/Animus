using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Text;
using System.Diagnostics;

namespace EventReceivers.tabZadania
{
    public class tabZadania : SPItemEventReceiver
    {
        // cmdFormatka
        private const string _CMD_ZAPISZ_WYNIKI = "Zapisz wyniki na karcie kontrolnej";
        private const string _CMD_ZATWIERDZ_I_WYSLIJ = "Zatwierdź wyniki i wyślij do klienta";
        private const string _CMD_ZATWIERDZ_I_ZAKONCZ = "Zatwierdź wyniki i zakończ zadanie";
        private const string _CMD_WYCOFAJ = "Wycofaj z karty kontrolnej";

        // enumStatusZadania
        private const string _ZADANIE_GOTOWE = "Gotowe";
        private const string _ZADANIE_ZWOLNIONE = "Zwolnione do wysyłki";
        private const string _ZADANIE_OBSLUGA = "Obsługa";
        private static string _ZADANIE_ZAKONCZONE = "Zakończone";

        public override void ItemAdded(SPItemEventProperties properties)
        {
            Execute(properties);
        }

        public override void ItemUpdated(SPItemEventProperties properties)
        {
            Execute(properties);
        }

        private void Execute(SPItemEventProperties properties)
        {
            Debug.WriteLine("EventReceivers.tabZadania.Execute");

            this.EventFiringEnabled = false;

            SPListItem item = properties.ListItem;

            BLL.Logger.LogEvent_EventReceiverInitiated(item);

            try
            {
                Set_Up(item);
                Update_Resources(item);
                Update_Title(item);
                Update_Extras(item);

                if (BLL.Tools.Get_Text(item, "cmdFormatka").Length > 0
                | BLL.Tools.Get_Text(item, "cmdFormatka_Wiadomosc").Length > 0
                | BLL.Tools.Get_Text(item, "cmdFormatka_Zadanie").Length > 0)
                {
                    Debug.WriteLine("wybrano akcję na formatce");

                    switch (item.ContentType.Name)
                    {

                        case "Rozliczenie ZUS":
                            Manage_ZUS(item);
                            break;
                        case "Rozliczenie podatku dochodowego":
                            Manage_PD(item);
                            break;
                        case "Rozliczenie podatku dochodowego spółki":
                            Manage_PDS(item);
                            break;
                        case "Rozliczenie podatku VAT":
                            Manage_VAT(item);
                            break;
                        case "Rozliczenie z biurem rachunkowym":
                            Manage_RBR(item);
                            break;

                        case "Zadanie":
                            Manage_Zadanie(item);
                            break;

                        case "Wiadomość z ręki":
                        case "Wiadomość z szablonu":
                        case "Wiadomość grupowa":
                        case "Wiadomość grupowa z szablonu":
                        case "Informacja uzupełniająca":
                            BLL.tabWiadomosci.CreateMailMessage(item);
                            break;

                        default:
                            new Exception("Nieobsługiwany typ zadania " + item.ContentType.Name);
                            break;
                    }

                    Reset_CMD(item);
                }
                else
                {
                    //aktualizuj informacje o dostarczeniu dokumentów na karcie kontrolnej
                    if (item.ContentType.Name == "Rozliczenie podatku dochodowego"
                        | item.ContentType.Name == "Rozliczenie podatku dochodowego spółki")
                    {
                        BLL.tabKartyKontrolne.Update_POD(item);
                    }
                }

                item.SystemUpdate();

                this.EventFiringEnabled = true;

                BLL.Logger.LogEvent_EventReceiverCompleted(item);

            }
            catch (Exception ex)
            {
#if DEBUG
                throw new NotImplementedException(ex.ToString());
#else
                BLL.Tools.Set_Text(item, "enumStatusZadania", "Anulowane");
                item.SystemUpdate();
                BLL.Logger.LogError(properties.WebUrl,
                    string.Format("klient:{0}, {1}", BLL.Tools.Get_LookupId(item, "selKlient"), ex.ToString()));
                var result = ElasticEmail.EmailGenerator.ReportError(ex, properties.WebUrl.ToString());
#endif
            }

        }

        private void Update_Title(SPListItem item)
        {
            if (string.IsNullOrEmpty(item.Title))
            {
                BLL.Tools.Set_Text(item, "Title", BLL.Tools.Get_LookupValue(item, "selProcedura"));
            }
        }

        private void Reset_CMD(SPListItem item)
        {
            Debug.WriteLine("EventReceivers.tabZadania.tabZadania.Reset_CMD");

            BLL.Tools.Set_Text(item, "cmdFormatka", string.Empty);
            BLL.Tools.Set_Text(item, "cmdFormatka_Wiadomosc", string.Empty);
            BLL.Tools.Set_Text(item, "cmdFormatka_Zadanie", string.Empty);
        }

        private void Update_Resources(SPListItem item)
        {
            //obsługa procedury

            int procId = BLL.Tools.Get_LookupId(item, "selProcedura");
            if (procId == 0) //aktualizuj procedurę
            {
                switch (item.ContentType.Name)
                {
                    case "Wiadomość z ręki":
                    case "Wiadomość z szablonu":
                    case "Wiadomość grupowa":
                    case "Wiadomość grupowa z szablonu":
                    case "Informacja uzupełniająca":
                        procId = BLL.tabProcedury.Ensure(item.Web, ": " + item.ContentType.Name, true);
                        break;
                    default:
                        //przypisz procedurę na podstawie tematu
                        procId = BLL.tabProcedury.Ensure(item.Web, item.Title, false);
                        break;
                }

                //update procedura
                BLL.Tools.Set_Value(item, "selProcedura", (int)procId);
            }

            if (procId > 0)
            {
                //update termin realizacji
                Set_TerminRealizacji(ref item, procId);

                //update operatora
                Set_Operator(ref item, procId);
            }
        }

        private void Update_Extras(SPListItem item)
        {
            //operator
            item = Set_OperatorUser(item);
        }

        private void Set_Operator(ref SPListItem item, int procId)
        {
            if (procId > 0 && item["selOperator"] == null)
            {
                int operatorId = BLL.tabProcedury.Get_OperatorById(item.Web, procId);
                if (operatorId > 0)
                {
                    item["selOperator"] = operatorId;
                }
            }
        }

        private void Set_TerminRealizacji(ref SPListItem item, int procId)
        {
            if (procId > 0 && (item["colTerminRealizacji"] == null || (DateTime)item["colTerminRealizacji"] != new DateTime()))
            {

                int termin = BLL.tabProcedury.Get_TerminRealizacjiOfsetById(item.Web, procId);
                if (termin > 0)
                {
                    item["colTerminRealizacji"] = DateTime.Today.AddDays(termin);
                }
            }
        }



        private SPListItem Set_OperatorUser(SPListItem item)
        {
            int operatorId = BLL.Tools.Get_LookupId(item, "selOperator");
            if (operatorId > 0)
            {
                int userId = BLL.dicOperatorzy.Get_UserIdById(item.Web, operatorId);
                BLL.Tools.Set_Value(item, "_KontoOperatora", userId);
            }
            else
            {
                BLL.Tools.Set_Value(item, "_KontoOperatora", 0);
            }
            return item;
        }

        #region ZUS
        private void Manage_ZUS(SPListItem item)
        {
            Debug.WriteLine("EventReceivers.tabZadania.tabZadania.Manage_ZUS");

            string cmd = BLL.Tools.Get_Text(item, "cmdFormatka");
            if (string.IsNullOrEmpty(cmd)) return;

            if (IsValid_ZUS_Form(item))
            {
                if (IsValid_ZUS_MessageDetails(item))
                {
                    Update_StatusZadania(item, cmd);
                    BLL.tabKartyKontrolne.Update_ZUS_Data(item);
                }
            }
        }

        private bool IsValid_ZUS_Form(SPListItem item)
        {
            bool result = true;
            StringBuilder errLog = new StringBuilder();

            //Składki ZUS
            bool zp = BLL.Tools.Get_Flag(item, "colZatrudniaPracownikow");
            string fo = BLL.Tools.Get_Text(item, "colFormaOpodakowania_ZUS");
            switch (fo)
            {
                case "Tylko zdrowotna":
                    if (true)
                    {
                        if (zp)
                        {
                            if (HasValue(item, "colZUS_SP_Skladka")
                                && HasValue(item, "colZUS_ZD_Skladka")
                                && HasValue(item, "colZUS_FP_Skladka"))
                            { }
                            else
                            {
                                errLog.AppendLine("Nieprawidłowa warotść składki");
                                result = false;
                            }
                        }
                        else
                        {
                            if (HasValue(item, "colZUS_ZD_Skladka"))
                            { }
                            else
                            {
                                errLog.AppendLine("Nieprawidłowa warotść składki zdrowotnej");
                                result = false;

                                BLL.Tools.Set_Value(item, "colZUS_SP_Skladka", 0);
                                BLL.Tools.Set_Value(item, "colZUS_FP_Skladka", 0);
                            }
                        }
                    }
                    break;

                case "Tylko pracownicy":
                    if (!zp)
                    {
                        zp = true;
                        BLL.Tools.Set_Flag(item, "colZatrudniaPracownikow", zp);
                    }

                    if (HasValue(item, "colZUS_SP_Skladka")
                        & HasValue(item, "colZUS_ZD_Skladka")
                        & HasValue(item, "colZUS_FP_Skladka"))
                    { }
                    else
                    {
                        errLog.AppendLine("Nieprawidłowa warotść składki");
                        return result;
                    }


                    break;

                default:
                    if (HasValue(item, "colZUS_SP_Skladka")
                        & HasValue(item, "colZUS_ZD_Skladka")
                        & HasValue(item, "colZUS_FP_Skladka"))
                    { }
                    else
                    {
                        errLog.AppendLine("Nieprawidłowa warotść składki");
                        result = false;
                    }

                    break;

            }

            //PIT-4R
            if (result && zp)
            {
                bool pit4R = BLL.Tools.Get_Flag(item, "colZUS_PIT-4R_Zalaczony");
                if (pit4R)
                {
                    if (HasValue(item, "colZUS_PIT-4R"))
                    { }
                    else
                    {
                        errLog.AppendLine("Nieprawidłowa warotść PIT-4R");
                        result = false;
                    }
                }
                else BLL.Tools.Set_Value(item, "colZUS_PIT-4R", 0);
            }

            //PIT-8AR
            if (result && zp)
            {
                bool pit8AR = BLL.Tools.Get_Flag(item, "colZUS_PIT-8AR_Zalaczony");
                if (pit8AR)
                {
                    if (HasValue(item, "colZUS_PIT-8AR"))
                    { }
                    else
                    {
                        errLog.AppendLine("Nieprawidłowa warotść PIT-8AR");
                        result = false;
                    }
                }
                else BLL.Tools.Set_Value(item, "colZUS_PIT-8AR", 0);
            }

            if (!zp)
            {
                BLL.Tools.Set_Flag(item, "colZUS_PIT-4R_Zalaczony", false);
                BLL.Tools.Set_Value(item, "colZUS_PIT-4R", 0);
                BLL.Tools.Set_Flag(item, "colZUS_PIT-8AR_Zalaczony", false);
                BLL.Tools.Set_Value(item, "colZUS_PIT-8AR", 0);
            }


            //Załączniki
            bool lpz = BLL.Tools.Get_Flag(item, "colZUS_ListaPlac_Zalaczona");
            bool rz = BLL.Tools.Get_Flag(item, "colZUS_Rachunki_Zalaczone");

            if (result && lpz && rz)
            {
                if (item.Attachments.Count >= 2)
                { }
                else
                {
                    errLog.AppendLine("Brak załączników");
                    result = false;
                }

            }
            else if (result && (lpz || rz))
            {
                if (item.Attachments.Count >= 1)
                { }
                else
                {
                    errLog.AppendLine("Brak załączników");
                    result = false;
                }

            }

            Update_ValidationInfo(item, result, errLog);

            return result;
        }

        private static void Update_ValidationInfo(SPListItem item, bool result, StringBuilder errLog)
        {
            if (!result)
            {
                //ustaw flagę walidacji
                BLL.Tools.AppendNote_Top(item, "colNotatka", errLog.ToString(), true);
                BLL.Tools.Set_Flag(item, "_Validation", true);
            }
            else
            {
                //wyczyść flagę walidacji jeżeli ustawiona
                if (BLL.Tools.Get_Flag(item, "_Validation"))
                {
                    BLL.Tools.Set_Flag(item, "_Validation", false);
                }
            }
        }

        private bool IsValid_ZUS_MessageDetails(SPListItem item)
        {
            bool result = true;
            StringBuilder errLog = new StringBuilder();

            if (!HasIndex(item, "selOddzialZUS"))
            {
                errLog.AppendLine("Brak przypisania oddziału ZUS");
                result = false;
            };


            if (!HasText(item, "colFormaOpodakowania_ZUS"))
            {
                errLog.AppendLine("Brak informacji o formie opodatkowania");
                result = false;
            };



            if (!HasDate(item, "colZUS_TerminPlatnosciSkladek"))
            {
                errLog.AppendLine("Brak informacji o terminie płatności składek");
                result = false;
            };

            Update_ValidationInfo(item, result, errLog);

            return result;
        }


        #endregion

        #region PD
        private void Manage_PD(SPListItem item)
        {
            Debug.WriteLine("EventReceivers.tabZadania.tabZadania.Manage_PD");

            string cmd = BLL.Tools.Get_Text(item, "cmdFormatka");
            if (string.IsNullOrEmpty(cmd)) return;

            if (IsValid_PD_Form(item))
            {
                if (IsValid_PD_MessageDetails(item))
                {
                    Update_StatusZadania(item, cmd);

                    BLL.tabKartyKontrolne.Update_PD_Data(item);
                }
            }
            else
            {

            }
        }

        private static void Update_StatusZadania(SPListItem item, string cmd)
        {
            Debug.WriteLine("EventReceivers.tabZadania.tabZadania.Update_StatusZadania");
            switch (cmd)
            {
                case _CMD_ZAPISZ_WYNIKI:
                    BLL.Tools.Set_Text(item, "enumStatusZadania", _ZADANIE_GOTOWE);
                    break;
                case _CMD_ZATWIERDZ_I_WYSLIJ:
                    BLL.Tools.Set_Text(item, "enumStatusZadania", _ZADANIE_ZWOLNIONE);
                    break;
                case _CMD_ZATWIERDZ_I_ZAKONCZ:
                    BLL.Tools.Set_Text(item, "enumStatusZadania", _ZADANIE_ZAKONCZONE);
                    break;
                case _CMD_WYCOFAJ:
                    BLL.Tools.Set_Text(item, "enumStatusZadania", _ZADANIE_OBSLUGA);
                    break;
            }

            Debug.WriteLine("enumStatusZadania=" + BLL.Tools.Get_Text(item, "enumStatusZadania"));
        }

        private bool IsValid_PD_Form(SPListItem item)
        {
            bool result = true;
            StringBuilder errLog = new StringBuilder();

            switch (BLL.Tools.Get_Text(item, "colPD_OcenaWyniku"))
            {
                case "Dochód":
                    if (HasValue(item, "colPD_WartoscDochodu")
                        & HasValue(item, "colPD_WartoscDoZaplaty"))
                    {
                        BLL.Tools.Set_Value(item, "colPD_WartoscStraty", 0);
                    }
                    else
                    {
                        errLog.AppendLine("Nieprawidłowa wartość dochodu lub do zapłaty");
                        result = false;
                    }

                    break;
                case "Strata":
                    if (HasValue(item, "colPD_WartoscStraty"))
                    {
                        BLL.Tools.Set_Value(item, "colPD_WartoscDochodu", 0);
                        BLL.Tools.Set_Value(item, "colPD_WartoscDoZaplaty", 0);
                    }
                    else
                    {
                        errLog.AppendLine("Nieprawidłowa wartość straty");
                        result = false;
                    }


                    break;
                default:
                    errLog.AppendLine("Nieprawidłowa wartość oceny wyniku");
                    result = false;
                    break;
            }

            if (result)
            {
                BLL.Tools.Set_Flag(item, "colPotwierdzenieOdbioruDokumentow", true);
            }

            Update_ValidationInfo(item, result, errLog);

            return result;
        }

        private bool IsValid_PD_MessageDetails(SPListItem item)
        {
            bool result = true;
            StringBuilder errLog = new StringBuilder();

            if (!HasIndex(item, "selUrzadSkarbowy"))
            {
                errLog.AppendLine("Brak przypisania urzędu skarbowego");
                result = false;
            };

            if (!HasText(item, "colFormaOpodatkowaniaPD"))
            {
                errLog.AppendLine("Brak informacji o formie opodatkowania");
                result = false;
            };

            if (!HasText(item, "enumRozliczeniePD"))
            {
                errLog.AppendLine("Brak informacji o sposobie rozliczenia podatku");
                result = false;
            };

            Update_ValidationInfo(item, result, errLog);

            return result;
        }


        #endregion

        #region PDS
        private void Manage_PDS(SPListItem item)
        {
            Debug.WriteLine("EventReceivers.tabZadania.tabZadania.Manage_PDS");

            string cmd = BLL.Tools.Get_Text(item, "cmdFormatka");
            if (string.IsNullOrEmpty(cmd)) return;

            if (IsValid_PDS_Form(item))
            {
                if (IsValid_PDS_MessageDetails(item))
                {
                    Update_StatusZadania(item, cmd);
                }
            }
            else
            {

            }
        }

        private bool IsValid_PDS_Form(SPListItem item)
        {
            //todo: throw new NotImplementedException();
            return true;
        }

        private bool IsValid_PDS_MessageDetails(SPListItem item)
        {
            //todo: throw new NotImplementedException();
            return true;
        }
        #endregion

        #region VAT
        private void Manage_VAT(SPListItem item)
        {
            Debug.WriteLine("EventReceivers.tabZadania.tabZadania.Manage_VAT");

            string cmd = BLL.Tools.Get_Text(item, "cmdFormatka");
            if (string.IsNullOrEmpty(cmd)) return;

            if (IsValid_VAT_Form(item))
            {
                if (IsValid_VAT_MessageDetails(item))
                {
                    Update_StatusZadania(item, cmd);

                    BLL.tabKartyKontrolne.Update_VAT_Data(item);
                }
            }
            else
            {

            }
        }

        private bool IsValid_VAT_Form(SPListItem item)
        {
            bool result = true;
            StringBuilder errLog = new StringBuilder();

            switch (BLL.Tools.Get_Text(item, "colVAT_Decyzja"))
            {
                case "Do zapłaty":
                    if (HasValue(item, "colVAT_WartoscDoZaplaty"))
                    {
                        BLL.Tools.Set_Value(item, "colVAT_WartoscDoPrzeniesienia", 0);
                        BLL.Tools.Set_Value(item, "colVAT_WartoscDoZwrotu", 0);
                    }
                    else
                    {
                        errLog.AppendLine("Nieprawidłowa wartość do zapłaty");
                        result = false;
                    }





                    break;
                case "Do przeniesienia":
                    if (HasValue(item, "colVAT_WartoscDoPrzeniesienia"))
                    {
                        BLL.Tools.Set_Value(item, "colVAT_WartoscDoZaplaty", 0);
                        BLL.Tools.Set_Value(item, "colVAT_WartoscDoZwrotu", 0);
                    }
                    else
                    {
                        errLog.AppendLine("Nieprawidłowa wartość do przeniesienia");
                        result = false;
                    }

                    break;
                case "Do zwrotu":
                    if (HasValue(item, "colVAT_WartoscDoZwrotu"))
                    {
                        BLL.Tools.Set_Value(item, "colVAT_WartoscDoZaplaty", 0);
                        BLL.Tools.Set_Value(item, "colVAT_WartoscDoPrzeniesienia", 0);
                    }
                    else
                    {
                        errLog.AppendLine("Nieprawidłowa wartość do zwrotu");
                        result = false;
                    }

                    if (!HasText(item, "colVAT_TerminZwrotuPodatku"))
                    {
                        errLog.AppendLine("brak informacji o terminie zwrotu podatku");
                        result = false;
                    }

                    break;
                case "Do przeniesienia i do zwrotu":
                    if (HasValue(item, "colVAT_WartoscDoPrzeniesienia")
                        & HasValue(item, "colVAT_WartoscDoZwrotu"))
                    {
                        BLL.Tools.Set_Value(item, "colVAT_WartoscDoZaplaty", 0);
                    }
                    else
                    {
                        errLog.AppendLine("Nieprawidłowa wartość do przeniesienia lub do zwrotu");
                    }

                    if (!HasText(item, "colVAT_TerminZwrotuPodatku"))
                    {
                        errLog.AppendLine("brak informacji o terminie zwrotu podatku");
                        result = false;
                    }


                    break;
                default:

                    errLog.AppendLine("Nieprawidłowa decyzja dotycząca rozliczenia VAT");
                    result = false;
                    break;
            }

            if (!HasValue(item, "colVAT_WartoscNadwyzkiZaPoprzedniMiesiac"))
            {
                errLog.AppendLine("Nieprawidłowa wartość nadwyżki a poprzedni miesiąc");
                result = false;
            }

            Update_ValidationInfo(item, result, errLog);

            return result;
        }



        private bool IsValid_VAT_MessageDetails(SPListItem item)
        {
            bool result = true;
            StringBuilder errLog = new StringBuilder();

            if (!HasIndex(item, "selUrzadSkarbowy"))
            {
                errLog.AppendLine("Brak przypisania urzędu skarbowego");
                result = false;
            };

            if (!HasText(item, "colFormaOpodatkowaniaVAT"))
            {
                errLog.AppendLine("Brak informacji o formie opodatkowania");
                result = false;
            };

            if (!HasText(item, "enumRozliczenieVAT"))
            {
                errLog.AppendLine("Brak informacji o sposobie rozliczenia podatku VAT");
                result = false;
            };


            Update_ValidationInfo(item, result, errLog);

            return result;
        }
        #endregion

        #region RBR
        private void Manage_RBR(SPListItem item)
        {
            Debug.WriteLine("EventReceivers.tabZadania.tabZadania.Manage_RBR");

            string cmd = BLL.Tools.Get_Text(item, "cmdFormatka");
            if (string.IsNullOrEmpty(cmd)) return;

            if (IsValid_RBR_Form(item))
            {
                if (IsValid_RBR_MessageDetails(item))
                {
                    Update_StatusZadania(item, cmd);

                    BLL.tabKartyKontrolne.Update_RBR_Data(item);
                }
            }
            else
            {

            }
        }

        private bool IsValid_RBR_Form(SPListItem item)
        {
            bool result = true;
            StringBuilder errLog = new StringBuilder();

            result = result && HasDate(item, "colBR_DataWystawieniaFaktury");
            if (!result) errLog.AppendLine("Nieprawidłowa wartość dochodu lub do zapłaty");

            result = result && HasText(item, "colBR_NumerFaktury");
            if (!result) errLog.AppendLine("Nieprawidłowa wartość dochodu lub do zapłaty");

            result = result && HasValue(item, "colBR_WartoscDoZaplaty");
            if (!result) errLog.AppendLine("Nieprawidłowa wartość dochodu lub do zapłaty");

            result = result && HasDate(item, "colBR_TerminPlatnosci");
            if (!result) errLog.AppendLine("Nieprawidłowa wartość dochodu lub do zapłaty");

            Update_ValidationInfo(item, result, errLog);

            return result;
        }

        private bool IsValid_RBR_MessageDetails(SPListItem item)
        {
            return true;
        }
        #endregion

        private void Manage_Zadanie(SPListItem item)
        {
            Debug.WriteLine("EventReceivers.tabZadania.tabZadania.Manage_Zadanie");

            //todo: throw new NotImplementedException();
        }

        private static void Set_Up(SPListItem item)
        {
            //status
            if (BLL.Tools.Get_Text(item, "enumStatusZadania").Equals("Nowe")
                && BLL.Tools.Get_Date(item, "Created").CompareTo(BLL.Tools.Get_Date(item, "Modified")) != 0)
            {
                BLL.Tools.Set_Text(item, "enumStatusZadania", "Obsługa");
                item.SystemUpdate();
            }

            BLL.Tools.Set_Text(item, "_Validation", string.Empty);
        }

        private bool HasValue(SPListItem item, string col)
        {
            double v = BLL.Tools.Get_Value(item, col);
            if (v >= 0) return true;
            else return false;
        }

        private bool HasIndex(SPListItem item, string col)
        {
            int idx = BLL.Tools.Get_LookupId(item, col);
            if (idx > 0) return true;
            else return false;
        }

        private bool HasDate(SPListItem item, string col)
        {
            DateTime d = BLL.Tools.Get_Date(item, col);
            if (d > new DateTime()) return true;
            else return false;
        }

        private bool HasText(SPListItem item, string col)
        {
            string s = BLL.Tools.Get_Text(item, col);
            if (!string.IsNullOrEmpty(s)) return true;
            else return false;
        }

    }




}
