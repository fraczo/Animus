using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace EventReceivers.tabKlienci
{

    public class tabKlienci : SPItemEventReceiver
    {
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
            this.EventFiringEnabled = false;
            SPListItem item = properties.ListItem;
            BLL.Logger.LogEvent_EventReceiverInitiated(item);

            try
            {
                Cleanup(item);
                Set_NazwaPrezentowana(item);
                Update_Serwisy(item);
                Update_Extensions(item);
                //Update_FolderInLibrary(item, web);

                item.SystemUpdate();                
            }
            catch (Exception ex)
            {
                BLL.Logger.LogEvent(properties.WebUrl, ex.ToString());
                var result = ElasticEmail.EmailGenerator.ReportError(ex, properties.WebUrl.ToString());
            }
            finally
            {
                BLL.Logger.LogEvent_EventReceiverCompleted(item);
                this.EventFiringEnabled = true;
            }
        }

        private void Cleanup(SPListItem item)
        {
            item["colOsobaDoKontaktu"] = BLL.Tools.Cleanup_Text(item, "colOsobaDoKontaktu");
            item["colNazwaSkrocona"] = BLL.Tools.Cleanup_Text(item, "colNazwaSkrocona");
            item["colNazwaFirmy"] = BLL.Tools.Cleanup_Text(item, "colNazwaFirmy");
            item["colTelefon"] = BLL.Tools.Cleanup_Text(item, "colTelefon");
            item["colMiejscowosc"] = BLL.Tools.Cleanup_Text(item, "colMiejscowosc");

            // mail podstawowy
            string email = BLL.Tools.Get_Text(item, "colEmail");
            if (BLL.Tools.IsValidEmail(email))
            {
                item["colEmail"] = email;
            }
            else
            {
                if (!string.IsNullOrEmpty(email))
                {
                    item["colEmail"] = string.Empty;
                    string memo = string.Format("{0} nie jest poprawnym adresem i został usunięty z kartoteki klienta", email);
                    //ToDo: wyślij wiadomość do operatora
                }
            }

            // mail uzupełniający
            email = BLL.Tools.Get_Text(item, "colEmail2");
            if (BLL.Tools.IsValidEmail(email))
            {
                item["colEmail2"] = email;
            }
            else
            {
                if (!string.IsNullOrEmpty(email))
                {
                    item["colEmail2"] = string.Empty;
                    string memo = string.Format("{0} nie jest poprawnym adresem i został usunięty z kartoteki klienta", email);
                    //ToDo: wyślij wiadomość do operatora
                }
            }
        }
        private void Set_NazwaPrezentowana(SPListItem item)
        {
            switch (item.ContentType.Name)
            {
                case "KPiR":
                    Set_NazwaPrezentowana_Firma(item);
                    break;
                case "Osoba fizyczna":
                    Set_NazwaPrezentowana_OsobaFizyczna(item);
                    break;
                default:
                    break;
            }
        }

        private static void Set_NazwaPrezentowana_Firma(SPListItem item)
        {
            string result = BLL.Tools.Get_Text(item, "colNazwaSkrocona");
            string nip = BLL.Tools.Get_Text(item, "colNIP");
            if (!string.IsNullOrEmpty(nip))
            {
                result = result + " NIP:" + nip;
            }

            item["_NazwaPrezentowana"] = result.Trim();
        }

        private static void Set_NazwaPrezentowana_OsobaFizyczna(SPListItem item)
        {
            string firma = BLL.Tools.Get_LookupValue(item, "selKlient_NazwaSkrocona");

            string result = BLL.Tools.Get_Text(item, "colNazwisko");
            string imie = BLL.Tools.Get_Text(item, "colImie");
            if (!string.IsNullOrEmpty(imie))
            {
                result = result + ", " + imie;
            }

            string pesel = BLL.Tools.Get_Text(item, "colPESEL");
            if (!string.IsNullOrEmpty(pesel))
            {
                result = result + " PESEL:" + pesel;
            }

            item["_NazwaPrezentowana"] = firma + @" | " + result.Trim();
        }

        private void Update_Serwisy(SPListItem item)
        {
            Update_Serwisy_PD(item);
            Update_Serwisy_VAT(item);
            Update_Serwisy_ZUS(item);
            Update_Serwisy_ZP_NZP(item); //zatrudnia pracowników
            Update_Serwisy_AD(item); //audyt danych
            Update_Serwisy_GBW(item); //generowanie blankietu wpłaty
            Update_Serwisy_RBR_RB(item); //rozliczenie z biurem rachunkowym
            Update_Serwisy_POT(item); //przypomnienia o terminie płatności
            Update_Serwisy_WKB(item); //wymagany kontakt bezpośredni

        }

        private void Update_Serwisy_WKB(SPListItem item)
        {
            BLL.Tools.Remove_Services(ref item, "selSewisy", "WKB");

            switch (item.ContentType.Name)
            {
                case "KPiR":
                case "Osoba fizyczna":

                //jeżeli adres email jest nieprawidłowy lub wskazana jest inna preferowana forma komunikacji    
                if (!BLL.Tools.IsValidEmail(BLL.Tools.Get_Text(item, "colEmail"))
                    || BLL.Tools.IsSelectorAssigned(item, "enumPreferowanaFormaKomunikacji", "Email"))
                    {
                        BLL.Tools.Assign_Service(ref item, "selSewisy", "WKB");
                    }

                    break;
                default:
                    break;
            }
        }

        private void Update_Serwisy_POT(SPListItem item)
        {
            BLL.Tools.Remove_Services(ref item, "selSewisy", "POT");

            switch (item.ContentType.Name)
            {
                case "KPiR":
                case "Osoba fizyczna":

                    if (BLL.Tools.Get_Flag(item, "colPrzypomnienieOTerminiePlatnosci"))
                    {
                        BLL.Tools.Assign_Service(ref item, "selSewisy", "POT");
                    }

                    break;
                default:
                    break;
            }
        }

        private void Update_Serwisy_RBR_RB(SPListItem item)
        {
            BLL.Tools.Remove_Services(ref item, "selSewisy", "RBR");
            BLL.Tools.Remove_Services(ref item, "selSewisy", "RB");

            switch (item.ContentType.Name)
            {
                case "KPiR":
                    if (BLL.Tools.Get_LookupValue(item, "selTerminPlatnosci").Equals("Gotówka"))
                    {
                        BLL.Tools.Assign_Service(ref item, "selSewisy", "RB");
                        
                        // dodaj klienta w tablicy stawek
                        BLL.tabStawki.Ensure_KlientExist(item.Web, item.ID);
                    }
                    else
                    {
                        if (BLL.Tools.Get_Value(item, "colOplataMiesieczna") > 0)
                        {
                            BLL.Tools.Assign_Service(ref item, "selSewisy", "RBR");
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        private void Update_Serwisy_GBW(SPListItem item)
        {
            BLL.Tools.Remove_Services(ref item, "selSewisy", "GBW");

            switch (item.ContentType.Name)
            {
                case "KPiR":
                case "Osoba fizyczna":

                    if (BLL.Tools.Get_Flag(item, "colDrukWplaty"))
                    {
                        BLL.Tools.Assign_Service(ref item, "selSewisy", "GBW");
                    }

                    break;
                default:
                    break;
            }
        }

        private void Update_Serwisy_AD(SPListItem item)
        {
            BLL.Tools.Remove_Services(ref item, "selSewisy", "AD");

            switch (item.ContentType.Name)
            {
                case "KPiR":
                case "Osoba fizyczna":

                    if (BLL.Tools.Get_Flag(item, "colAudytDanych"))
                    {
                        BLL.Tools.Assign_Service(ref item, "selSewisy", "AD");
                    }

                    break;
                default:
                    break;
            }
        }

        private void Update_Serwisy_ZP_NZP(SPListItem item)
        {
            BLL.Tools.Remove_Services(ref item, "selSewisy", "ZP");
            BLL.Tools.Remove_Services(ref item, "selSewisy", "NZP");

            switch (item.ContentType.Name)
            {
                case "KPiR":
                    if (BLL.Tools.Get_Flag(item, "colZatrudniaPracownikow"))
                    {
                        BLL.Tools.Assign_Service(ref item, "selSewisy", "ZP");
                    }
                    else
                    {
                        BLL.Tools.Assign_Service(ref item, "selSewisy", "NZP");
                    }

                    break;
                default:
                    break;
            }
        }

        private void Update_Serwisy_PD(SPListItem item)
        {

            BLL.Tools.Remove_Services(ref item, "selSewisy", "PD-*");
            BLL.Tools.Remove_Services(ref item, "selSewisy", "PDS-*");

            switch (item.ContentType.Name)
            {
                case "KPiR":
                case "Osoba fizyczna":

                    if (BLL.Tools.IsSelectorAssigned(item, "selUrzadSkarbowy", string.Empty)
                        && BLL.Tools.IsSelectorAssigned(item, "colFormaOpodatkowaniaPD_KPiR", "Nie dotyczy")
                        && BLL.Tools.IsSelectorAssigned(item, "enumRozliczeniePD", "Nie dotyczy"))
                    {
                        switch (BLL.Tools.Get_Text(item, "enumRozliczeniePD"))
                        {
                            case "Miesięcznie":
                                BLL.Tools.Assign_Service(ref item, "selSewisy", "PD-M");
                                break;
                            case "Kwartalnie":
                                BLL.Tools.Assign_Service(ref item, "selSewisy", "PD-KW");
                                break;

                            default:
                                break;
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        private void Update_Serwisy_VAT(SPListItem item)
        {
            BLL.Tools.Remove_Services(ref item, "selSewisy", "VAT-*");

            switch (item.ContentType.Name)
            {
                case "KPiR":
                    if ((BLL.Tools.IsSelectorAssigned(item, "selUrzadSkarbowyVAT", string.Empty)
                        || BLL.Tools.IsSelectorAssigned(item, "selUrzadSkarbowy", string.Empty))
                        && BLL.Tools.IsSelectorAssigned(item, "enumRozliczenieVAT", "Nie dotyczy"))
                    {
                        switch (BLL.Tools.Get_Text(item, "enumRozliczenieVAT"))
                        {
                            case "Miesięcznie":
                                BLL.Tools.Assign_Service(ref item, "selSewisy", "VAT-M");
                                break;
                            case "Kwartalnie":
                                BLL.Tools.Assign_Service(ref item, "selSewisy", "VAT-KW");
                                break;

                            default:
                                break;
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        private void Update_Serwisy_ZUS(SPListItem item)
        {
            BLL.Tools.Remove_Services(ref item, "selSewisy", "ZUS-*");

            switch (item.ContentType.Name)
            {
                case "KPiR":
                case "Osoba fizyczna":
                    if (BLL.Tools.IsSelectorAssigned(item, "selOddzialZUS", string.Empty)
                        && BLL.Tools.IsSelectorAssigned(item, "colFormaOpodakowania_ZUS", "Nie dotyczy"))
                    {
                        switch (BLL.Tools.Get_Text(item, "colFormaOpodakowania_ZUS"))
                        {
                            case "Mały ZUS":
                                BLL.Tools.Assign_Service(ref item, "selSewisy", "ZUS-M");
                                break;
                            case "Mały ZUS + Chorobowa":
                                BLL.Tools.Assign_Service(ref item, "selSewisy", "ZUS-M+C");
                                break;
                            case "Duży ZUS":
                                BLL.Tools.Assign_Service(ref item, "selSewisy", "ZUS-D");
                                break;
                            case "Duży ZUS + Chorobowa":
                                BLL.Tools.Assign_Service(ref item, "selSewisy", "ZUS-D+C");
                                break;
                            case "Tylko zdrowotna":
                                BLL.Tools.Assign_Service(ref item, "selSewisy", "ZUS-ZD");
                                break;
                            case "Tylko pracownicy":
                                BLL.Tools.Assign_Service(ref item, "selSewisy", "ZUS-PRAC");
                                break;

                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private static void Update_Extensions(SPListItem item)
        {
            // aktualizacja odwołań do lookupów
            item["_TypZawartosci"] = item.ContentType.Name;
            item["_Biuro"] = BLL.Tools.Get_LookupValue(item, "selBiuro");
            item["_ZatrudniaPracownikow"] = BLL.Tools.Get_Flag(item, "colZatrudniaPracownikow").ToString();

            if (item["selDedykowanyOperator_Podatki"] != null)
            {
                item["_DedykowanyOperator_Podatki"] = BLL.Tools.Get_LookupValue(item,"selDedykowanyOperator_Podatki");
            }
            if (item["selDedykowanyOperator_Kadry"] != null)
            {
                item["_DedykowanyOperator_Kadry"] = BLL.Tools.Get_LookupValue(item,"selDedykowanyOperator_Kadry");
            }
            if (item["selDedykowanyOperator_Audyt"] != null)
            {
                item["_DedykowanyOperator_Audyt"] = BLL.Tools.Get_LookupValue(item,"selDedykowanyOperator_Audyt");
            }
        }
        //private static void Update_FolderInLibrary(SPListItem item, SPWeb web)
        //{
        //    string typKlienta = item["ContentType"].ToString();
        //    switch (typKlienta)
        //    {
        //        case "KPiR":
        //        case "KSH":
        //            string folderName = item["colNazwaSkrocona"] != null ? item["colNazwaSkrocona"].ToString() : string.Empty;
        //            string status = item["enumStatus"] != null ? item["enumStatus"].ToString() : string.Empty;

        //            if (status == "Aktywny" && !String.IsNullOrEmpty(folderName))
        //            {
        //                int docId = BLL.libDokumenty.Ensure_FolderExist(web, folderName);
        //                int currDocId = item["_DocumentId"] != null ? int.Parse(item["_DocumentId"].ToString()) : 0;

        //                if (docId > 0 && currDocId != docId)
        //                {
        //                    item["_DocumentId"] = docId.ToString();
        //                    item.SystemUpdate();
        //                }
        //            }
        //            break;

        //        default:
        //            break;
        //    }
        //}



    }
}
