using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace BLL
{
    public class tabWiadomosci
    {
        const string targetList = "Wiadomości";

        public static void AddNew(SPWeb web, SPListItem item, string nadawca, string odbiorca, string kopiaDla, bool KopiaDoNadawcy, bool KopiaDoBiura, string temat, string tresc, string trescHTML, DateTime planowanaDataNadania, int zadanieId, int klientId)
        {
            AddNew(web, item, nadawca, odbiorca, kopiaDla, KopiaDoNadawcy, KopiaDoBiura, temat, tresc, trescHTML, planowanaDataNadania, item.ID, klientId, BLL.Models.Marker.Ignore);
        }

        public static void AddNew(SPWeb web, SPListItem item, string nadawca, string odbiorca, string kopiaDla, bool KopiaDoNadawcy, bool KopiaDoBiura, string temat, string tresc, string trescHTML, DateTime planowanaDataNadania, int zadanieId, int klientId, BLL.Models.Marker marker)
        {
            SPList list = web.Lists.TryGetList(targetList);
            SPListItem newItem = list.AddItem();
            newItem["Title"] = temat;
            if (string.IsNullOrEmpty(nadawca)) nadawca = BLL.admSetup.GetValue(web, "EMAIL_BIURA");

            newItem["colNadawca"] = nadawca;
            newItem["colOdbiorca"] = odbiorca;
            newItem["colKopiaDla"] = kopiaDla;
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
        }

        /// <summary>
        /// tworzy zlecenie wysyłki wiadomości bez załączników (nie przekazuje item)
        /// </summary>
        public static void AddNew(SPWeb web, string nadawca, string odbiorca, string kopiaDla, bool KopiaDoNadawcy, bool KopiaDoBiura, string temat, string tresc, string trescHTML, DateTime planowanaDataNadania, int zadanieId, int klientId)
        {
            AddNew(web, null, nadawca, odbiorca, kopiaDla, KopiaDoNadawcy, KopiaDoBiura, temat, tresc, trescHTML, planowanaDataNadania, zadanieId, klientId);
        }

        private static void AddNew(SPListItem item, DateTime reminderDate, string subject, string bodyHtml)
        {
            int klientId = BLL.Tools.Get_LookupId(item, "selKlient");
            string nadawca = string.Empty;
            string odbiorca = BLL.Tools.Get_Text(item, "colEmail");
            AddNew(item.Web, nadawca, odbiorca, string.Empty, false, false, subject, string.Empty, bodyHtml, reminderDate, item.ID, klientId);
        }

        public static void CreateMailMessage(SPListItem item)
        {
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
                    default:
                        break;
                }

                BLL.Tools.Set_Text(ref item, "cmdFormatka_Wiadomosc", string.Empty);
                item.SystemUpdate();
            }
        }

        private static void CreateMailMessage_Wiadomosc(SPListItem item, int klientId, string subject, string bodyHTML)
        {
#if DEBUG
            Logger.LogEvent("CreateMailMessage_Wiadomosc " + item.ContentType.Name + " z:" + item.ID.ToString() + " k:" + klientId.ToString(), string.Empty);
#endif
            string cmd = BLL.Tools.Get_Text(item, "cmdFormatka_Wiadomosc");

            if (!string.IsNullOrEmpty(cmd))
            {


                string kopiaDla = string.Empty;
                bool KopiaDoNadawcy = false;
                bool KopiaDoBiura = false;

                string nadawca = BLL.Tools.Get_CurrentUser(item);

                if (cmd == "Wyślij z kopią do mnie") KopiaDoNadawcy = true;

                // przygotuj wiadomość
                string temat = string.Empty;
                string tresc = string.Empty;
                string trescHTML = string.Empty;

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
                            BLL.tabWiadomosci.AddNew(item.Web, item, nadawca, odbiorca, kopiaDla, KopiaDoNadawcy, KopiaDoBiura, temat, tresc, trescHTML, BLL.Tools.Get_Date(item, "colPlanowanaDataNadania"), item.ID, klientId);
                            BLL.Tools.Set_Text(ref item, "enumStatusZadania", "Wysyłka");
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
                            BLL.tabWiadomosci.AddNew(item.Web, item, nadawca, odbiorca, kopiaDla, KopiaDoNadawcy, KopiaDoBiura, temat, tresc, trescHTML, new DateTime(), 0, 0);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private static void CreateMailMessage_WiadomoscZReki(SPListItem item, int klientId)
        {
#if DEBUG
            Logger.LogEvent("CreateMailMessage_WiadomoscZReki", item.ID.ToString());
#endif
            string bodyHTML = BLL.Tools.Get_Text(item, "colTresc");
            //string subject = BLL.Tools.Get_Text(item, "colTematWiadomosci");
            string subject = item.Title;
            CreateMailMessage_Wiadomosc(item, klientId, subject, bodyHTML);
        }

        private static void CreateMailMessage_WiadomoscZSzablonu(SPListItem item, int klientId)
        {
#if DEBUG
            Logger.LogEvent("CreateMailMessage_WiadomoscZSzablonu", item.ID.ToString());
#endif
            int szablonId = BLL.Tools.Get_LookupId(item, "selSzablonWiadomosci");
            string bodyHTML = BLL.Tools.Get_Text(item, "colInformacjaDlaKlienta");
            string subject = string.Empty;
            BLL.tabSzablonyWiadomosci.GetSzablonId(item.Web, szablonId, out subject, ref bodyHTML);

            CreateMailMessage_Wiadomosc(item, klientId, subject, bodyHTML);
        }

        private static void CreateMailMessage_WiadomoscDoGrupy(SPListItem item)
        {
#if DEBUG
            Logger.LogEvent("CreateMailMessage_WiadomoscDoGrupy", item.ID.ToString());
#endif
            string cmd = BLL.Tools.Get_Text(item, "cmdFormatka_Wiadomosc");

            if (!string.IsNullOrEmpty(cmd) && cmd == "Wyślij wiadomość testową")
            {
                CreateMailMessage_WiadomoscZReki(item, 0);
            }
            else
            {
                Array klientListItems = BLL.tabKlienci.Get_WybraniKlienci(item);
                foreach (SPListItem klientItem in klientListItems)
                {
                    CreateMailMessage_WiadomoscZReki(item, klientItem.ID);
                }
            }
        }

        private static void CreateMailMessage_WiadomoscDoGrupyZSzablonu(SPListItem item)
        {
#if DEBUG
            Logger.LogEvent("CreateMailMessage_WiadomoscDoGrupyZSzablonu", item.ID.ToString());
#endif

            string cmd = BLL.Tools.Get_Text(item, "cmdFormatka_Wiadomosc");

            if (!string.IsNullOrEmpty(cmd) && cmd == "Wyślij wiadomość testową")
            {
                CreateMailMessage_WiadomoscZSzablonu(item, 0);
            }
            else
            {
                Array klientListItems = BLL.tabKlienci.Get_WybraniKlienci(item);
                foreach (SPListItem klientItem in klientListItems)
                {
                    int klientId = BLL.Tools.Get_LookupId(item, "selKlient");
                    CreateMailMessage_WiadomoscZSzablonu(item, klientItem.ID);
                }
            }
        }

    }
}
