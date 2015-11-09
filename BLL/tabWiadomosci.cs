using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Net.Mail;
using System.IO;

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
                                    Copy_Attachement(newItem, file);
                                break;
                            case BLL.Models.Marker.ReminderZUS_PIT:
                                if (file.Name.StartsWith("DRUK WPŁATY__PIT"))
                                    Copy_Attachement(newItem, file);
                                break;
                            default:
                                Copy_Attachement(newItem, file);
                                break;
                        }


                    }
                }
            }

            newItem.SystemUpdate();
        }

        private static void Copy_Attachement(SPListItem newItem, SPFile file)
        {
            int bufferSize = 20480;
            byte[] byteBuffer = new byte[bufferSize];
            byteBuffer = file.OpenBinary();
            newItem.Attachments.Add(file.Name, byteBuffer);
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
            int klientId = Get_KlientId(item);
            string nadawca = string.Empty;
            string odbiorca = Get_String(item, "colEmail");
            AddNew(item.Web, nadawca, odbiorca, string.Empty, false, false, subject, string.Empty, bodyHtml, reminderDate, item.ID, klientId);
        }

        private static string Get_String(SPListItem item, string col)
        {
            return item[col] != null ? item[col].ToString() : string.Empty;
        }

        #region Helpers
        private static int Get_KlientId(SPListItem item)
        {
            string col = "selKlient";
            return item[col] != null ? new SPFieldLookupValue(item[col].ToString()).LookupId : 0;
        }
        #endregion

        public static void CreateMailMessage(SPListItem item)
        {
            string cmd = BLL.Tools.Get_Text(item, "cmdFormatka_Wiadomosc");
            if (!string.IsNullOrEmpty(cmd))
            {
                switch (item.ContentType.Name)
                {
                    case "Wiadomość z ręki":
                        CreateMailMessage_WiadomoscZReki(item);
                        break;
                    case "Wiadomość z szablonu":
                        CreateMailMessage_WiadomoscZSzablonu(item);
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
            }
        }

        private static void CreateMailMessage_WiadomoscZReki(SPListItem item)
        {
            int klientId = BLL.Tools.Get_LookupId(item, "selKlient");
            if (klientId > 0)
            {
                string odbiorca = BLL.tabKlienci.Get_EmailById(item.Web, klientId);
                if (BLL.Tools.Is_ValidEmail(odbiorca))
                {
                    string kopiaDla = string.Empty;
                    bool KopiaDoNadawcy = false;
                    bool KopiaDoBiura = false;

                    string nadawca = BLL.Tools.Get_CurrentUser(item);
                    string cmd = BLL.Tools.Get_Text(item, "cmdFormatka_Wiadomosc");
                    if (cmd == "Wyślij z kopią do mnie")
                    {
                        KopiaDoNadawcy = true;
                    }

                    if (cmd == "Wyślij"
                        || cmd == "Wyślij z kopią do mnie")
                    {
                        // przygotuj wiadomość
                        string temat = string.Empty;
                        string tresc = string.Empty;
                        string trescHTML = string.Empty;

                        BLL.dicSzablonyKomunikacji.Get_TemplateByKod(item, "EMAIL_DEFAULT_BODY.Include", out temat, out trescHTML, nadawca);
                        temat = item.Title;
                        trescHTML = trescHTML.Replace("___BODY___", BLL.Tools.Get_Text(item, "colTresc"));
                        

                        BLL.tabWiadomosci.AddNew(item.Web, item, nadawca, odbiorca, kopiaDla, KopiaDoNadawcy, KopiaDoBiura, temat, tresc, trescHTML, BLL.Tools.Get_Date(item, "colPlanowanaDataNadania"), item.ID, klientId);
                    }


                }
            }

        }

        private static void CreateMailMessage_WiadomoscZSzablonu(SPListItem item)
        {
            throw new NotImplementedException();
        }

        private static void CreateMailMessage_WiadomoscDoGrupy(SPListItem item)
        {
            throw new NotImplementedException();
        }

        private static void CreateMailMessage_WiadomoscDoGrupyZSzablonu(SPListItem item)
        {
            throw new NotImplementedException();
        }
    }
}
