using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BLL
{
    public class tabWiadomosci
    {
        const string targetList = "Wiadomości";
        private const string _INFO_HTML_TEMPLATE_NAME = "INFO_TEMPLATE";

        public static int AddNew(SPWeb web, SPListItem item, string nadawca, string odbiorca, string kopiaDla, bool KopiaDoNadawcy, bool KopiaDoBiura, string temat, string tresc, string trescHTML, DateTime planowanaDataNadania, int zadanieId, int klientId, int kartaKontrolnaId, BLL.Models.Marker marker)
        {
            SPList list = web.Lists.TryGetList(targetList);
            SPListItem newItem = list.AddItem();
            newItem["Title"] = temat;
            if (string.IsNullOrEmpty(nadawca)) nadawca = BLL.admSetup.GetValue(web, "EMAIL_BIURA");

            newItem["colNadawca"] = nadawca;
            newItem["colOdbiorca"] = odbiorca;

            if (!string.IsNullOrEmpty(kopiaDla))
            {
                newItem["colKopiaDla"] = kopiaDla;
            }

            newItem["colTresc"] = tresc;
            newItem["colTrescHTML"] = trescHTML;
            if (!string.IsNullOrEmpty(planowanaDataNadania.ToString()) && planowanaDataNadania != new DateTime())
            {
                newItem["colPlanowanaDataNadania"] = planowanaDataNadania.ToString();
            }
            newItem["colKopiaDoNadawcy"] = KopiaDoNadawcy;
            newItem["colKopiaDoBiura"] = KopiaDoBiura;
            if (zadanieId > 0) newItem["_ZadanieId"] = zadanieId;

            if (klientId > 0) newItem["selKlient_NazwaSkrocona"] = klientId;

            if (kartaKontrolnaId > 0) newItem["_KartaKontrolnaId"] = kartaKontrolnaId;


            //newItem.SystemUpdate();

            //obsługa wysyłki załączników jeżeli Item został przekazany w wywołaniu procedury
            if (item != null)
            {
                for (int attachmentIndex = 0; attachmentIndex < item.Attachments.Count; attachmentIndex++)
                {
                    string url = item.Attachments.UrlPrefix + item.Attachments[attachmentIndex];
                    SPFile file = item.ParentList.ParentWeb.GetFile(url);

                    if (file.Exists)
                    {
                        //sprawdź markety i dodawaj tylko odpowiednie pliki
                        switch (marker)
                        {
                            case BLL.Models.Marker.ReminderZUS:
                                if (file.Name.StartsWith("DRUK WPŁATY__ZUS")
                                    || file.Name.StartsWith("DRUK WPŁATY__Składka zdrowotna"))
                                    BLL.Tools.Copy_Attachement(newItem, file);
                                break;
                            case BLL.Models.Marker.ReminderZUS_PIT:
                                if (file.Name.StartsWith("DRUK WPŁATY__PIT"))
                                    BLL.Tools.Copy_Attachement(newItem, file);
                                break;
                            default:
                                BLL.Tools.Copy_Attachement(newItem, file);
                                break;
                        }


                    }
                }
            }

            newItem.SystemUpdate();

            return newItem.ID;
        }

        private static void AddNew(SPListItem item, DateTime reminderDate, string subject, string bodyHtml)
        {
            int klientId = BLL.Tools.Get_LookupId(item, "selKlient");
            string nadawca = string.Empty;
            string odbiorca = BLL.Tools.Get_Text(item, "colEmail");
            AddNew(item.Web, item, nadawca, odbiorca, string.Empty, false, false, subject, string.Empty, bodyHtml, reminderDate, item.ID, klientId, 0, BLL.Models.Marker.Ignore);
        }

        public static void CreateMailMessage(SPListItem item)
        {
            Debug.WriteLine("BLL.tabWiadomosci.CreateMailMessage: " + item.ContentType.Name);

            string cmd = BLL.Tools.Get_Text(item, "cmdFormatka_Wiadomosc");
            if (!string.IsNullOrEmpty(cmd))
            {
                int klientId;

                switch (item.ContentType.Name)
                {
                    case "Wiadomość z ręki":
                        klientId = BLL.Tools.Get_LookupId(item, "selKlient");
                        CreateMailMessage_WiadomoscZReki(item, klientId);
                        break;
                    case "Wiadomość z szablonu":
                        klientId = BLL.Tools.Get_LookupId(item, "selKlient");
                        CreateMailMessage_WiadomoscZSzablonu(item, klientId);
                        break;
                    case "Wiadomość grupowa":
                        CreateMailMessage_WiadomoscDoGrupy(item);
                        break;
                    case "Wiadomość grupowa z szablonu":
                        CreateMailMessage_WiadomoscDoGrupyZSzablonu(item);
                        break;
                    case "Informacja uzupełniająca":
                        klientId = BLL.Tools.Get_LookupId(item, "selKlient");
                        CreateMailMessage_InformacjaUzupelniajaca(item, klientId);
                        break;
                    default:
                        break;
                }

                BLL.Tools.Set_Text(item, "cmdFormatka_Wiadomosc", string.Empty);
                item.SystemUpdate();
            }
        }

        private static void CreateMailMessage_InformacjaUzupelniajaca(SPListItem item, int klientId)
        {
            Debug.WriteLine("BLL.tabWiadomosci.CreateMailMessage_InformacjaUzupelniajaca");

            //subject
            string subjectTemplate = @":: Informacja uzupełniająca na koniec {0} dla {1}";
            string subject = string.Format(subjectTemplate,
                                BLL.Tools.Get_LookupValue(item, "selOkres"),
                                BLL.Tools.Get_LookupValue(item, "selKlient"));

            //body
            string trTemplate = BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, "TR_TEMPLATE.Include");
            StringBuilder rows = new StringBuilder();

            AppendTR(ref rows, item, "colPrzychod", "Przychód", trTemplate);
            AppendTR(ref rows, item, "colKoszty", "Koszty", trTemplate);

            //dochód/strata

            switch (BLL.Tools.Get_Text(item, "colPD_OcenaWyniku"))
            {
                case "Dochód":
                    AppendTR(ref rows, item, "colPD_WartoscDochodu", "Dochód", trTemplate);
                    break;
                case "Strata":
                    AppendTR(ref rows, item, "colPD_WartoscStraty", "Strata", trTemplate);
                    break;
                default:
                    break;
            }

            //Obrót

            switch (BLL.Tools.Get_Text(item, "colObrot_Opcja"))
            {
                case "Do kasy fiskalnej":
                    AppendTR(ref rows, item, "colObrotDoKasyFiskalnej", "Obrót do kasy fiskalnej", trTemplate);
                    break;
                case "Do VAT":
                    AppendTR(ref rows, item, "colObrotDoVAT", "Obrót do VAT", trTemplate);
                    break;
                default:
                    break;
            }

            StringBuilder table = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, "TABLE_TEMPLATE"));
            table.Replace("[[ROWS]]", rows.ToString());

            // komentarz

            StringBuilder body = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, "INFO_TEMPLATE"));
            body.Replace("[[Tytul]]", "Wyniki finansowe - informacja uzupełniająca");
            body.Replace("[[TABLE]]", table.ToString());
            body.Replace("[[Tresc]]", BLL.Tools.Get_Text(item, "colTresc"));

            StringBuilder sbINFO = new StringBuilder(BLL.dicSzablonyKomunikacji.Ensure_HTMLByKod(item.Web, _INFO_HTML_TEMPLATE_NAME));


            CreateMailMessage_Wiadomosc(item, klientId, subject, body.ToString());
        }

        private static void AppendTR(ref StringBuilder rows, SPListItem item, string col, string desc, string trTemplate)
        {
            double v = BLL.Tools.Get_Value(item, col);
            if (v > 0)
            {
                StringBuilder r = new StringBuilder(trTemplate);
                r.Replace("[[Opis]]", desc);
                r.Replace("[[Wartosc]]", BLL.Tools.Format_Currency(v));
                rows.Append(r.ToString());
            }
        }



        private static void CreateMailMessage_Wiadomosc(SPListItem item, int klientId, string subject, string bodyHTML)
        {
            Debug.WriteLine("BLL.tabWiadomosci.CreateMailMessage_Wiadomosc");

            string cmd = BLL.Tools.Get_Text(item, "cmdFormatka_Wiadomosc");

            if (!string.IsNullOrEmpty(cmd))
            {
                // adresowanie wiadomości

                string kopiaDla = string.Empty;
                kopiaDla = BLL.Tools.Append_EmailCC(item.Web, klientId, kopiaDla);
                bool KopiaDoNadawcy = false;
                bool KopiaDoBiura = false;



                if (cmd == "Wyślij z kopią do mnie") KopiaDoNadawcy = true;

                // przygotuj wiadomość
                string temat = string.Empty;
                string tresc = string.Empty;
                string trescHTML = string.Empty;

                //string nadawca = BLL.Tools.Get_CurrentUser(item); - wymusza przypisanie stopki operatora na podstawie aktualnego adresu nadawcy
                string nadawca = string.Empty; // wymusza przypisanie stopki operatora na podstawie aktualnie wybranego operatora

                BLL.dicSzablonyKomunikacji.Get_TemplateByKod(item, "EMAIL_DEFAULT_BODY.Include", out temat, out trescHTML, nadawca);
                temat = subject;
                trescHTML = trescHTML.Replace("___BODY___", bodyHTML);

                switch (cmd)
                {
                    case "Wyślij":
                    case "Wyślij z kopią do mnie":

                        string odbiorca = BLL.tabKlienci.Get_EmailById(item.Web, klientId);
                        if (BLL.Tools.Is_ValidEmail(odbiorca))
                        {
                            BLL.tabWiadomosci.AddNew(item.Web, item, nadawca, odbiorca, kopiaDla, KopiaDoNadawcy, KopiaDoBiura, temat, tresc, trescHTML, BLL.Tools.Get_Date(item, "colPlanowanaDataNadania"), item.ID, klientId, 0, BLL.Models.Marker.WithAttachements);
                            BLL.Tools.Set_Text(item, "enumStatusZadania", "Wysyłka");
                            item.SystemUpdate();
                        }
                        break;
                    case "Wyślij wiadomość testową":

                        temat = string.Format(@"::TEST::{0}", temat.ToString());
                        kopiaDla = string.Empty;
                        KopiaDoNadawcy = false;
                        KopiaDoBiura = false;

                        odbiorca = BLL.Tools.Get_CurrentUser(item);
                        if (BLL.Tools.Is_ValidEmail(odbiorca))
                        {
                            BLL.tabWiadomosci.AddNew(item.Web, item, nadawca, odbiorca, kopiaDla, KopiaDoNadawcy, KopiaDoBiura, temat, tresc, trescHTML, new DateTime(), 0, 0, 0, Models.Marker.WithAttachements);
                        }
                        break;
                    default:
                        break;
                }
            }
        }


        private static void CreateMailMessage_WiadomoscZReki(SPListItem item, int klientId)
        {
            Debug.WriteLine("BLL.tabWiadomosci.CreateMailMessage_WiadomoscZReki");

            string bodyHTML = BLL.Tools.Get_Text(item, "colTresc");
            //string subject = BLL.Tools.Get_Text(item, "colTematWiadomosci");
            string subject = item.Title;
            CreateMailMessage_Wiadomosc(item, klientId, subject, bodyHTML);
        }

        private static void CreateMailMessage_WiadomoscZSzablonu(SPListItem item, int klientId)
        {
            Debug.WriteLine("BLL.tabWiadomosci.CreateMailMessage_WiadomoscZSzablonu");

            int szablonId = BLL.Tools.Get_LookupId(item, "selSzablonWiadomosci");
            string bodyHTML = BLL.Tools.Get_Text(item, "colInformacjaDlaKlienta");
            string subject = string.Empty;
            string funkcjeSzablonu = string.Empty;
            BLL.tabSzablonyWiadomosci.GetSzablonId(item.Web, szablonId, out subject, ref bodyHTML, out funkcjeSzablonu);

            //obsługa funkcji specjalnych

            if (!string.IsNullOrEmpty(funkcjeSzablonu))
            {
                Debug.WriteLine("Aktywne funkcje szablonu: " + funkcjeSzablonu);

                Array funkcje = Regex.Split(funkcjeSzablonu, ";#");
                foreach (string f in funkcje)
                {
                    switch (f)
                    {
                        case "Weryfikuj POD":
                            if (!_IsAllowedToSendPODReminder(item.Web, klientId))
                            {
                                Debug.WriteLine("_IsAllowedToSendPODReminder=false");
                                return;
                            }
                            else
                            {
                                Debug.WriteLine("_IsAllowedToSendPODReminder=true");
                            }
                            break;
                        case "Zastąp markery":
                            _ReplaceKnownMarkers(bodyHTML, klientId);
                            break;
                        default:
                            break;
                    }
                }
            }

            CreateMailMessage_Wiadomosc(item, klientId, subject, bodyHTML);
        }

        private static void _ReplaceKnownMarkers(string bodyHTML, int klientId)
        {
            //todo: throw new NotImplementedException();
        }

        private static bool _IsAllowedToSendPODReminder(SPWeb web, int klientId)
        {
            // dla zadanego klineta sprawdź czy ma ustawiony Email jako preferowaną formę komunikacji i czy ma adres mailowy

            Models.Klient iok = new Models.Klient(web, klientId);
            if (iok.PreferowanaFormaKontaktu.Equals("Email")
                && !string.IsNullOrEmpty(iok.Email)) { }
            else return false;

            // dla bieżącej daty poszukaj ostatniego aktywnego okresu

            int targetOkresId = BLL.tabOkresy.Get_ActiveOkresId(web);
            if (targetOkresId > 0) { }
            else return false;

            // dla okresu odszukaj w kartach kontrolnych rekord klineta i sprawdź wartość flagi potwierdzenia otrzymania dokumentów

            SPListItem kk = BLL.tabKartyKontrolne.Get_KK_ByKlientId_ByOkresId(web, klientId, targetOkresId);
            if (kk != null)
            {
                // jeżeli flaga ustawiona -> zablokuj wysyłkę

                if (BLL.Tools.Get_Flag(kk, "colPotwierdzenieOdbioruDokumentow").Equals(true)) return false;

            }

            return true;

        }

        private static void CreateMailMessage_WiadomoscDoGrupy(SPListItem item)
        {
            Debug.WriteLine("BLL.tabWiadomosci.CreateMailMessage_WiadomoscDoGrupy");

            string cmd = BLL.Tools.Get_Text(item, "cmdFormatka_Wiadomosc");

            if (!string.IsNullOrEmpty(cmd) && cmd == "Wyślij wiadomość testową")
            {
                CreateMailMessage_WiadomoscZReki(item, 0);
            }
            else
            {
                Array klientListItems = BLL.tabKlienci.Get_WybraniKlienci(item);
                Debug.WriteLine("klienci: " + klientListItems.Length);

                foreach (SPListItem klientItem in klientListItems)
                {
                    Debug.WriteLine("KlientId: " + klientItem.ID.ToString());
                    CreateMailMessage_WiadomoscZReki(item, klientItem.ID);
                }
            }
        }

        private static void CreateMailMessage_WiadomoscDoGrupyZSzablonu(SPListItem item)
        {
            Debug.WriteLine("BLL.tabWiadomosci.CreateMailMessage_WiadomoscDoGrupyZSzablonu");

            string cmd = BLL.Tools.Get_Text(item, "cmdFormatka_Wiadomosc");

            if (!string.IsNullOrEmpty(cmd) && cmd == "Wyślij wiadomość testową")
            {
                CreateMailMessage_WiadomoscZSzablonu(item, 0);
            }
            else
            {
                Array klientListItems = BLL.tabKlienci.Get_WybraniKlienci(item);
                Debug.WriteLine("klienci: " + klientListItems.Length);

                foreach (SPListItem klientItem in klientListItems)
                {
                    Debug.WriteLine("KlientId: " + klientItem.ID.ToString());

                    int klientId = BLL.Tools.Get_LookupId(item, "selKlient");
                    CreateMailMessage_WiadomoscZSzablonu(item, klientItem.ID);
                }
            }
        }

        public static void Ensure_ColumnExist(SPWeb web, string col)
        {
            SPListItem item = web.Lists.TryGetList(targetList).Items.Add();
            BLL.Tools.Ensure_Column(item, col);
        }

        public static void Update_Komponenty(SPWeb web, int itemId, System.Collections.ArrayList komponenty)
        {
            SPListItem item = web.Lists.TryGetList(targetList).GetItemById(itemId);
            BLL.Tools.Set_SPFieldMultiChoiceValue(item, "_KomponentyKK", komponenty);
            item.SystemUpdate();
        }
    }
}
