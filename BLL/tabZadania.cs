using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using BLL.Models;
using System.IO;
using System.Diagnostics;

namespace BLL
{
    public class tabZadania
    {
        const string targetList = "Zadania"; // "tabZadania";
        private static object _STATUS_ZADANIA_ZAKONCZONE = "Zakończone";
        private static string _STATUS_ZADANIA_WYSYLKA = "Wysyłka";

        //public static string Define_KEY(SPItemEventDataCollection item)
        //{
        //    string result;
        //    string ct = item["ContentType"].ToString();

        //    if (ct == "Zadanie" || ct == "Element" || ct == "Folder")
        //    {
        //        return String.Empty;
        //    }

        //    int klientId = 0;
        //    int okresId = 0;

        //    if (item["selKlient"] != null)
        //    {
        //        klientId = new SPFieldLookupValue(item["selKlient"].ToString()).LookupId;
        //    }

        //    if (item["selOkres"] != null)
        //    {
        //        okresId = new SPFieldLookupValue(item["selOkres"].ToString()).LookupId;
        //    }

        //    result = String.Format(@"{0}:{1}:{2}",
        //        ct.ToString(),
        //        klientId.ToString(),
        //        okresId.ToString());

        //    return result;
        //}
        public static string Define_KEY(string ct, int klientId, int okresId)
        {
            string result;

            if (ct == "Zadanie" || ct == "Element" || ct == "Folder")
            {
                return String.Empty;
            }

            result = String.Format(@"{0}:{1}:{2}",
                ct.ToString(),
                klientId.ToString(),
                okresId.ToString());

            return result;
        }

        public static string Define_KEY(SPListItem item)
        {
            string ct = item["ContentType"].ToString();

            if (ct == "Zadanie" || ct == "Element" || ct == "Folder")
            {
                return String.Empty;
            }

            int klientId = 0;
            int okresId = 0;

            if (item["selKlient"] != null)
            {
                klientId = new SPFieldLookupValue(item["selKlient"].ToString()).LookupId;
            }

            if (item["selOkres"] != null)
            {
                okresId = new SPFieldLookupValue(item["selOkres"].ToString()).LookupId;
            }

            return Define_KEY(ct, klientId, okresId);
        }

        public static void Update_KEY(SPListItem item, string key)
        {

            string ct = item["ContentType"].ToString();

            if (item["KEY"] != null)
            {
                if (item["KEY"].ToString() != key)
                {
                    item["KEY"] = key;
                    item.SystemUpdate();
                }
            }
            else
            {
                item["KEY"] = key;
                item.SystemUpdate();
            }

            return;
        }

        /// <summary>
        /// zwraca identyfikator rekordu w tabZadania, który zawiera szukan klucz.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        public static bool Check_KEY_IsAllowed(string key, SPWeb web, int currentId)
        {
            bool result = true;

            var list = web.Lists.TryGetList(targetList);

            Array li = list.Items.Cast<SPListItem>()
                    .Where(i => i.ID != currentId)
                    .Where(i => i.ContentType.Name.StartsWith("Rozliczenie"))
                    .Where(i => BLL.Tools.Get_Text(i, "KEY").Equals(key))
                    .ToArray();

            if (li.Length > 0)
            {
                result = false;
            }

            return result;
        }



        public static void Create_ctZUS_Form(SPWeb web, string ct, int klientId, int okresId, string key, SPListItem klientItem, Klient iok)
        {
            Logger.LogEvent("Create_ctZUS_Form", klientId.ToString());

            SPList list = web.Lists.TryGetList(targetList);

            SPListItem item = list.AddItem();
            item["ContentType"] = ct;
            item["selKlient"] = klientId;
            item["selOkres"] = okresId;
            item["KEY"] = key;
            item["colZatrudniaPracownikow"] = iok.ZatrudniaPracownikow;

            //procedura
            string procName = string.Format(": {0}", ct);
            item["selProcedura"] = tabProcedury.Ensure(web, procName, true);
            item["Title"] = procName;

            BLL.Models.Okres o = new BLL.Models.Okres(web, okresId);

            //jeżeli ZUS-PRAC to nie wypełniaj wysokości składek
            if (!BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "ZUS-PRAC"))
            {

                if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "ZUS-M"))
                {
                    item["colZUS_SP_Skladka"] = o.Skladka_ZUS_M_SP;
                    item["colZUS_ZD_Skladka"] = o.Skladka_ZUS_M_ZD;
                    item["colZUS_FP_Skladka"] = o.Skladka_ZUS_M_FP;

                }
                else if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "ZUS-M+C"))
                {
                    item["colZUS_SP_Skladka"] = o.Skladka_ZUS_M_SPC;
                    item["colZUS_ZD_Skladka"] = o.Skladka_ZUS_M_ZD;
                    item["colZUS_FP_Skladka"] = o.Skladka_ZUS_M_FP;
                }
                else if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "ZUS-D"))
                {
                    item["colZUS_SP_Skladka"] = o.Skladka_ZUS_D_SP;
                    item["colZUS_ZD_Skladka"] = o.Skladka_ZUS_D_ZD;
                    item["colZUS_FP_Skladka"] = o.Skladka_ZUS_D_FP;
                }
                else if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "ZUS-D+C"))
                {
                    item["colZUS_SP_Skladka"] = o.Skladka_ZUS_D_SPC;
                    item["colZUS_ZD_Skladka"] = o.Skladka_ZUS_D_ZD;
                    item["colZUS_FP_Skladka"] = o.Skladka_ZUS_D_FP;
                }
                else if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "ZUS-ZD"))
                {
                    item["colZUS_ZD_Skladka"] = o.Skladka_ZUS_D_ZD;
                }
            }

            DateTime termin;

            // zatrudnia pracowników
            if (iok.ZatrudniaPracownikow)
            {
                termin = o.TerminPlatnosciSkladek_ZUS_ZPracownikami;
                //zeruj składki
                item["colZUS_SP_Skladka"] = 0;
                item["colZUS_ZD_Skladka"] = 0;
                item["colZUS_FP_Skladka"] = 0;
            }
            else
            {
                termin = o.TerminPlatnosciSkladek_ZUS_BezPracownikow; 
            }

            // zablokuj fundusz pracy
            if (iok.ZablokujFunduszPracy) item["colZUS_FP_Skladka"] = 0;

            int offset = (int)o.TerminPrzekazaniaWynikow_ZUS_Ofset;

            item["colZUS_TerminPlatnosciSkladek"] = termin;
            item["colTerminRealizacji"] = termin.AddDays(offset);

            //urząd skarbowy do podatku za pracowników
            item["selUrzadSkarbowy"] = iok.UrzadSkarbowyId;
            item["colPD_TerminPlatnosciPodatku"] = o.TerminPlatnosciPodatkuPD;


            //flagi
            item["colPrzypomnienieOTerminiePlatnos"] = iok.PrzypomnienieOTerminiePlatnosci;
            item["colDrukWplaty"] = iok.GenerowanieDrukuWplaty;
            item["colAudytDanych"] = iok.AudytDanych;

            //uwagi 
            item["colUwagiKadrowe"] = iok.UwagiKadrowe;

            if (iok.FormaOpodatkowaniaZUS != "Nie dotyczy")
            {
                item["colZUS_Opcja"] = iok.FormaOpodatkowaniaZUS;
            }
            item["colFormaOpodakowania_ZUS"] = iok.FormaOpodatkowaniaZUS;
            item["selOddzialZUS"] = iok.OddzialZUSId;

            //forma opodatkowania ZUS


            // przypisz domyślnego operatora
            int operatorId = iok.OperatorId_Kadry;
            if (operatorId > 0)
            {
                item["selOperator"] = operatorId;
                Set_KontoOperatora(item, operatorId);
            }

            item["enumStatusZadania"] = "Nowe";

            item.SystemUpdate();
        }

        

        public static int Get_NumerZadaniaBR(SPWeb web, int klientId, int okresId)
        {
            int result = 0;

            SPList list = web.Lists.TryGetList(targetList);
            SPListItem item = list.Items.Cast<SPListItem>()
                .Where(i => i["ContentType"].ToString() == @"Rozliczenie z biurem rachunkowym")
                .Where(i => new SPFieldLookupValue(i["selKlient"].ToString()).LookupId == klientId)
                .Where(i => new SPFieldLookupValue(i["selOkres"].ToString()).LookupId == okresId)
                .SingleOrDefault();

            if (item != null)
            {
                result = item.ID;
            }

            return result;
        }


        public static bool Add_FileFromURL(SPWeb web, int zadanieId, SPFile file)
        {
            bool result = false;
            string srcUrl = file.ServerRelativeUrl;

            SPList list = web.Lists.TryGetList(targetList);


            SPListItem item = list.GetItemById(zadanieId);

            if (item != null)
            {
                try
                {
                    srcUrl = web.Url + "/" + file.Url;

                    SPFile attachmentFile = web.GetFile(srcUrl);

                    //item.Attachments.Add(attachmentFile.Name, attachmentFile.OpenBinaryStream();


                    //FileStream fs = new FileStream(srcUrl, FileMode.Open, FileAccess.Read);

                    Stream fs = attachmentFile.OpenBinaryStream();

                    // Create a byte array of file stream length
                    byte[] buffer = new byte[fs.Length];

                    //Read block of bytes from stream into the byte array
                    fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));

                    //Close the File Stream
                    fs.Close();

                    item.Attachments.AddNow(attachmentFile.Name, buffer);

                    //aktualizuj informacje o załączonej fakturze
                    item["colBR_FakturaZalaczona"] = true;

                    item.SystemUpdate();

                }
                catch (Exception)
                {
                    //zabezpieczenie przed zdublowaniem plików
                }

            }

            return result;
        }

        public static void Update_InformacjeOWystawionejFakturze(SPWeb web, int zadanieId, string numerFaktury, double wartoscDoZaplaty, DateTime terminPlatnosci, DateTime dataWystawienia)
        {
            SPList list = web.Lists.TryGetList(targetList);
            SPListItem item = list.GetItemById(zadanieId);
            if (item != null)
            {
                item["colBR_NumerFaktury"] = numerFaktury;
                item["colBR_WartoscDoZaplaty"] = wartoscDoZaplaty;
                item["colBR_TerminPlatnosci"] = terminPlatnosci;
                item["colBR_DataWystawieniaFaktury"] = dataWystawienia;
                item.SystemUpdate();
            }
        }


        /// <summary>
        /// Aktualizuje informacje o wysyłce wyników do klienta
        /// Procedura wywoływana w procesu obsługi wiadomości po poprawnie zakończonej wysyłce
        /// </summary>
        public static void Update_StatusWysylki(SPWeb web, SPListItem messageItem, int zadanieId, StatusZadania statusZadania)
        {
            SPList list = web.Lists.TryGetList(targetList);
            SPListItem item = list.GetItemById(zadanieId);
            if (item != null)
            {
                string status = item["enumStatusZadania"] != null ? item["enumStatusZadania"].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(status)
                    && status == BLL.Models.StatusZadania.Wysyłka.ToString())
                {
                    //aktualizuj status i dodaj komentarz
                    item["enumStatusZadania"] = statusZadania.ToString();
                    string uwagi = item["colUwagi"] != null ? item["colUwagi"].ToString() : string.Empty;
                    uwagi = string.Format("{0} \n{1}",
                        uwagi,
                        messageItem.Title + " wysłane " + messageItem["Modified"].ToString() + " #" + messageItem.ID.ToString()).Trim();
                    item["colUwagi"] = uwagi;
                    item.SystemUpdate();
                }
            }

        }


        public static void Complete_PrzypomnienieOWysylceDokumentow(SPListItem item, int klientId, int okresId)
        {
            string KEY = Define_KEY("Prośba o dokumenty", klientId, okresId);
            if (!string.IsNullOrEmpty(KEY))
            {
                int taskId = Get_ZadanieByKEY(item.Web, KEY);
                if (taskId > 0)
                {
                    Set_Status(BLL.tabZadania.Get_ZadanieById(item.Web, taskId), "Zakończone");
                }
            }
        }

        public static SPListItem Get_ZadanieById(SPWeb web, int taskId)
        {
            SPList list = web.Lists.TryGetList(targetList);
            return list.GetItemById(taskId);
        }

        private static string Define_KEY(SPListItem item, string p)
        {
            throw new NotImplementedException();
        }

        private static void Set_Status(SPListItem item, string s)
        {
            string status = item["enumStatusZadania"] != null ? item["enumStatusZadania"].ToString() : string.Empty;
            if (status != s)
            {
                item["enumStatusZadania"] = s;
                item.SystemUpdate();
            }
        }

        private static int Get_ZadanieByKEY(SPWeb web, string KEY)
        {
            SPList list = web.Lists.TryGetList(targetList);
            SPListItem item = list.Items.Cast<SPListItem>()
                .Where(i => i["KEY"].ToString() == KEY)
                .FirstOrDefault();
            return item != null ? item.ID : 0;
        }


        public static void Update_PD_Status_DataWysylki(SPListItem item, DateTime date)
        {
            item["colPD_DataWylaniaInformacji"] = date;
            Update_StatusZadania_Zakonczone(item);
            item.SystemUpdate();
        }

        public static void Update_VAT_Status_DataWysylki(SPListItem item, DateTime date)
        {
            item["colVAT_DataWyslaniaInformacji"] = date;
            Update_StatusZadania_Zakonczone(item);
            item.SystemUpdate();
        }

        public static void Update_ZUS_Status_DataWysylki(SPListItem item, DateTime date)
        {
            item["colZUS_DataWyslaniaInformacji"] = date;
            Update_StatusZadania_Zakonczone(item);
            item.SystemUpdate();
        }

        public static void Update_RBR_Status_DataWysylki(SPListItem item, DateTime date)
        {
            item["colBR_DataPrzekazania"] = date;
            Update_StatusZadania_Zakonczone(item);
            item.SystemUpdate();
        }

        private static void Update_StatusZadania_Zakonczone(SPListItem item)
        {
            if (BLL.Tools.Get_Text(item, "enumStatusZadania").Equals(_STATUS_ZADANIA_WYSYLKA))
            {
                item["enumStatusZadania"] = _STATUS_ZADANIA_ZAKONCZONE;
            }
        }

        public static List<SPListItem> Get_ActiveTasksByContentType(SPWeb web, string ctName)
        {
            SPList list = web.Lists.TryGetList(targetList);

            List<SPListItem> results = (from SPListItem item in list.Items
                                        where item.ContentType.Name == ctName
                                        && (item["enumStatusZadania"].ToString() == "Nowe"
                                            || item["enumStatusZadania"].ToString() == "Obsługa")
                                        //&& Get_LookupValue(item, "selOperator") == "STAFix24 Robot"
                                        select item).ToList();
            return results;
        }

        #region Helpers
        private static string Get_LookupValue(SPListItem item, string col)
        {
            return item[col] != null ? new SPFieldLookupValue(item[col].ToString()).LookupValue : string.Empty;
        }
        #endregion

        public static void Set_ValidationFlag(SPListItem item, bool flag)
        {
            string targetColName = "_Validation";
            bool colFound = false;

            SPList list = item.Web.Lists.TryGetList(targetList);
            foreach (SPField col in list.Fields)
            {
                if (col.InternalName == targetColName)
                {
                    colFound = true;
                    break;
                }
            }

            if (!colFound)
            {
                //dodj kolumnę
                list.Fields.Add(targetColName, SPFieldType.Boolean, false);
                list.Update();
            }

            item[targetColName] = flag;
        }


        public static Array Get_GotoweZadaniaByProceduraId(SPWeb web, int proceduraId)
        {
            SPList list = web.Lists.TryGetList(targetList);
            Array result = list.Items.Cast<SPListItem>()
                .Where(i => i["enumStatusZadania"].ToString() == "Gotowe")
                .Where(i => i["selProcedura"] != null)
                .Where(i => new SPFieldLookupValue(i["selProcedura"].ToString()).LookupId == proceduraId)
                .ToArray();

            return result;
        }

        public static int Get_NumerZadaniaVAT(SPWeb web, int klientId, int okresId)
        {
            int result = 0;

            SPList list = web.Lists.TryGetList(targetList);
            SPListItem item = list.Items.Cast<SPListItem>()
                .Where(i => i.ContentType.Name == @"Rozliczenie podatku VAT")
                .Where(i => new SPFieldLookupValue(i["selKlient"].ToString()).LookupId == klientId)
                .Where(i => new SPFieldLookupValue(i["selOkres"].ToString()).LookupId == okresId)
                .SingleOrDefault();

            if (item != null)
            {
                result = item.ID;
            }

            return result;
        }

        public static Array Get_AktywneZadaniaByProceduraId(SPWeb web, int proceduraId)
        {
            SPList list = web.Lists.TryGetList(targetList);
            Array result = list.Items.Cast<SPListItem>()
                .Where(i => i["enumStatusZadania"].ToString() == "Nowe" || i["enumStatusZadania"].ToString() == "Obsługa")
                .Where(i => i["selProcedura"] != null)
                .Where(i => new SPFieldLookupValue(i["selProcedura"].ToString()).LookupId == proceduraId)
                .ToArray();

            return result;
        }

        public static void Create_ctPD_Form(SPWeb web, string ct, int klientId, int okresId, string key, SPListItem klientItem, Klient iok)
        {
            Logger.LogEvent("Create_ctPD_Form", klientId.ToString());

            string kod = string.Empty;

            if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "PD-M"))
            {
                kod = "PD-M";
            }
            else if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "PD-KW"))
            {
                kod = "PD-KW";
            }
            else return; // jeżeli żaden z powyższych to zakończ procedurę.

            SPList list = web.Lists.TryGetList(targetList);

            SPListItem item = list.AddItem();
            item["ContentType"] = ct;
            item["selKlient"] = klientId;
            item["selOkres"] = okresId;
            item["KEY"] = key;
            //procedura

            string procName = string.Format(": {0}", ct);
            item["selProcedura"] = tabProcedury.Ensure(web, procName, true);
            item["Title"] = procName;

            //numery kont i nazwa urzędu

            item["colPD_Konto"] = iok.NumerRachunkuPD;
            item["selUrzadSkarbowy"] = iok.UrzadSkarbowyId;

            //terminy

            BLL.Models.Okres o = new BLL.Models.Okres(web, okresId);

            switch (kod)
            {
                case "PD-M":
                    item["colPD_TerminPlatnosciPodatku"] = o.TerminPlatnosciPodatkuPD;
                    item["enumRozliczeniePD"] = "Miesięcznie";
                    break;
                case "PD-KW":
                    item["colPD_TerminPlatnosciPodatku"] = o.TerminPlatnosciPodatkuPD_KW;
                    item["enumRozliczeniePD"] = "Kwartalnie";
                    break;
                default:
                    break;
            }

            item["colTerminRealizacji"] = o.TerminPlatnosciPodatkuPD.AddDays((int)o.TerminPrzekazaniaWynikowPD_Ofset);

            //flagi

            item["colPrzypomnienieOTerminiePlatnos"] = iok.PrzypomnienieOTerminiePlatnosci;
            item["colDrukWplaty"] = iok.GenerowanieDrukuWplaty;
            item["colAudytDanych"] = iok.AudytDanych;
            item["colFormaOpodatkowaniaPD"] = iok.FormaOpodatkowaniaPD;

            //uwagi 
            item["colUwagiPD"] = iok.UwagiPD;

            //przypisz zadanie do domyślnego operatora
            int operatorId = iok.OperatorId_Podatki;
            if (operatorId > 0)
            {
                item["selOperator"] = operatorId;
                Set_KontoOperatora(item, operatorId);
            }

            item["enumStatusZadania"] = "Nowe";

            item.SystemUpdate();

        }

        private static void Set_KontoOperatora(SPListItem item, int operatorId)
        {
            item["_KontoOperatora"] = BLL.dicOperatorzy.Get_UserIdById(item.Web, operatorId);
        }

         public static void Create_ctVAT_Form(SPWeb web, string ct, int klientId, int okresId, string key, SPListItem klientItem, Klient iok)
        {
            Logger.LogEvent("Create_ctVAT_Form", klientId.ToString());

            string kod = string.Empty;

            if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "VAT-M"))
            {
                kod = "VAT-M";
            }
            else if (BLL.Tools.Has_SerwisAssigned(klientItem, "selSewisy", "VAT-KW"))
            {
                kod = "VAT-KW";
            }
            else return; // jeżeli żaden z powyższych to zakończ procedurę.


            SPList list = web.Lists.TryGetList(targetList);

            SPListItem item = list.AddItem();
            item["ContentType"] = ct;
            item["selKlient"] = klientId;
            item["selOkres"] = okresId;
            item["KEY"] = key;

            //procedura

            string procName = string.Format(": {0}", ct);
            item["selProcedura"] = tabProcedury.Ensure(web, procName, true);
            item["Title"] = procName;

            //numery kont i nazwa urzędu

            item["selUrzadSkarbowy"] = iok.UrzadSkarbowyVATId;
            item["colFormaOpodatkowaniaVAT"] = iok.FormaOpodatkowaniaVAT;

            //terminy
            BLL.Models.Okres o = new BLL.Models.Okres(web, okresId);
            int preOkresId = 0;

            switch (kod)
            {
                case "VAT-M":
                    item["colVAT_TerminPlatnosciPodatku"] = o.TerminPlatnosciPodatkuVAT;
                    item["enumRozliczenieVAT"] = "Miesięcznie";

                    preOkresId = BLL.tabOkresy.Get_PoprzedniOkresIdById(web, okresId);
                    break;
                case "VAT-KW":
                    item["colVAT_TerminPlatnosciPodatku"] = o.TerminPlatnosciPodatkuVAT_KW;
                    item["enumRozliczenieVAT"] = "Kwartalnie";

                    preOkresId = BLL.tabOkresy.Get_PoprzedniOkresKwartalnyIdById(web, okresId);
                    break;
                default:
                    break;
            }

            //przenieś wartość nadwyżki z poprzedniej deklaracji
            if (preOkresId > 0)
            {
                item["colVAT_WartoscNadwyzkiZaPoprzedniMiesiac"] = BLL.tabKartyKontrolne.Get_WartoscNadwyzkiDoPrzeniesienia(web, klientId, preOkresId);
            }

            item["colTerminRealizacji"] = o.TerminPlatnosciPodatkuVAT.AddDays((int)o.TerminPrzekazaniaWynikowVAT_Ofset);

            //flagi

            item["colPrzypomnienieOTerminiePlatnos"] = iok.PrzypomnienieOTerminiePlatnosci;
            item["colDrukWplaty"] = iok.GenerowanieDrukuWplaty;
            item["colAudytDanych"] = iok.AudytDanych;

            //uwagi 
            item["colUwagiVAT"] = iok.UwagiVAT;

            int operatorId = iok.OperatorId_Podatki;
            if (operatorId > 0)
            {
                item["selOperator"] = operatorId;
                Set_KontoOperatora(item, operatorId);
            }

            item["enumStatusZadania"] = "Nowe";

            item.SystemUpdate();

        }

        public static void Create_ctRBR_Form(SPWeb web, string ct, int klientId, int okresId, string key, SPListItem klientItem, Klient iok)
        {
            Logger.LogEvent("Create_ctRBR_Form", klientId.ToString());

            SPList list = web.Lists.TryGetList(targetList);

            SPListItem item = list.AddItem();
            item["ContentType"] = ct;
            item["selKlient"] = klientId;
            item["selOkres"] = okresId;
            item["KEY"] = key;

            //procedura

            string procName = string.Format(": {0}", ct);
            item["selProcedura"] = tabProcedury.Ensure(web, procName, true);
            item["Title"] = procName;

            //BLL.tabProcedury.Get_OperatorById(

            //numer konta biura

            Models.Okres o = new Okres(web, okresId);

            if (o.TerminPrzekazaniaRBR>new DateTime())
            {
                item["colTerminRealizacji"] = o.TerminPrzekazaniaRBR;
            }

            //flagi

            item["colPrzypomnienieOTerminiePlatnos"] = iok.PrzypomnienieOTerminiePlatnosci;
            item["colDrukWplaty"] = iok.GenerowanieDrukuWplaty;


            //zainicjowanie wartości domyślnych
            
            //data wystawienia faktury do 20 każdego miesiąca
            DateTime dataBazowa = o.DataZakonczenia.AddMonths(1);
            DateTime dataWystawieniaFaktury = new DateTime(dataBazowa.Year, dataBazowa.Month, 20);
            if (dataWystawieniaFaktury.DayOfWeek == DayOfWeek.Saturday) dataWystawieniaFaktury.AddDays(-1);
            if (dataWystawieniaFaktury.DayOfWeek == DayOfWeek.Sunday) dataWystawieniaFaktury.AddDays(-2);

            item["colBR_DataWystawieniaFaktury"] = dataWystawieniaFaktury;
            item["colBR_WartoscDoZaplaty"] = iok.OplataMiesieczna;
            item["colBR_TerminPlatnosci"] = dataWystawieniaFaktury.AddDays(iok.TerminPlatnosci);

            //uwagi 
            item["colUwagi"] = iok.Uwagi;

            //domyślny operator obsługujący podatki
            int operatorId = iok.OperatorId_Podatki;
            if (operatorId > 0)
            {
                item["selOperator"] = operatorId;
                Set_KontoOperatora(item, operatorId);
            }

            item["enumStatusZadania"] = "Nowe";

            item.SystemUpdate();
        }

        public static string Get_InfoDlaKlientaById(SPWeb web, int zadanieId)
        {
            SPListItem item = Get_ItemById(web, zadanieId);
            if (item != null) return BLL.Tools.Get_Text(item, "colInformacjaDlaKlienta").Trim();
            else return string.Empty;
        }

        /// <summary>
        /// dotyczy wyłącznie komendarza ZUSPD
        /// </summary>
        public static string Get_InfoDlaKlienta2ById(SPWeb web, int zadanieId)
        {
            SPListItem item = Get_ItemById(web, zadanieId);
            if (item != null) return BLL.Tools.Get_Text(item, "colInformacjaDlaKlienta2").Trim();
            else return string.Empty;
        }

        private static SPListItem Get_ItemById(SPWeb web, int zadanieId)
        {
            SPList list = web.Lists.TryGetList(targetList);
            return list.GetItemById(zadanieId);
        }

        public static void Update_StatusZadania(SPWeb web, int zadanieId, string value)
        {
            Debug.WriteLine("BLL.tabZadania.Update_StatusZadania");
            Debug.WriteLine("zadanieId=" + zadanieId.ToString());

            SPListItem item = web.Lists.TryGetList(targetList).GetItemById(zadanieId);
            BLL.Tools.Set_Text(item, "enumStatusZadania", value);
            item.SystemUpdate();
        }


        public static SPListItem GetItemById(SPWeb web, int itemId)
        {
            return web.Lists.TryGetList(targetList).Items.GetItemById(itemId);
        }

    }
}
