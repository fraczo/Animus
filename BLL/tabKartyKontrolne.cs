using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Diagnostics;

namespace BLL
{
    public class tabKartyKontrolne
    {
        const string targetList = "Karty kontrolne";
        const string colPOTWIERDZENIE_ODBIORU_DOKUMENTOW = "colPotwierdzenieOdbioruDokumento";
        private static object _STATUS_ZADANIA_ZWOLNIONE = "Zwolnione do wysyłki";

        public static int Ensure_KartaKontrolna(SPWeb web, int klientId, int okresId, BLL.Models.Klient iok)
        {
            Debug.WriteLine("BLL.tabKartyKontrolne.Ensure_KartaKontrolna");

            string KEY = Create_KEY(klientId, okresId);
            int formId = Get_KartaKontrolnaId(web, klientId, okresId, KEY, iok);

            if (formId > 0) return formId;
            else
            {
                return Create_KartaKontrolna(web, klientId, okresId, iok, KEY);
            }
        }

        private static int Create_KartaKontrolna(SPWeb web, int klientId, int okresId, BLL.Models.Klient iok, string KEY)
        {
            Debug.WriteLine("BLL.tabKartyKontrolne.Create_KartaKontrolna");

            SPListItem newItem = web.Lists.TryGetList(targetList).Items.Add();
            newItem["KEY"] = KEY;
            newItem["selKlient"] = klientId;
            newItem["selOkres"] = okresId;

            Set_KartaKontrolna_InitValues(newItem, iok);

            //ustaw CT
            if (iok.TypKlienta == "KSH") newItem["ContentType"] = "Karta kontrolna KSH";
            else newItem["ContentType"] = "Karta kontrolna KPiR";

            newItem.SystemUpdate();
            return newItem.ID;
        }

        public static void Update_PD_Data(Microsoft.SharePoint.SPListItem item)
        {
            string KEY = Create_KEY(item);
            int formId = Get_KartaKontrolnaId(item, KEY);

            SPListItem form = Get_KartaKontrolnaById(item.Web, formId);
            Copy_PDFields(item, form);
            form.SystemUpdate();
        }

        private static void Copy_PDFields(Microsoft.SharePoint.SPListItem item, SPListItem form)
        {
            Copy_Field(item, form, "colPotwierdzenieOdbioruDokumento");
            Copy_Field(item, form, "colFormaOpodatkowaniaPD");
            Copy_Field(item, form, "enumRozliczeniePD");
            Copy_Field(item, form, "colPD_OcenaWyniku");
            Copy_Field(item, form, "colPD_WartoscDochodu");
            Copy_Field(item, form, "colPD_WartoscDoZaplaty");
            Copy_Field(item, form, "colPD_WartoscStraty");

            Copy_Field(item, "selOperator", form, "selOperator_PD");
            Copy_Field(item, "colNotatka", form, "colUwagiPD");
            Copy_Id(item, form, "selZadanie_PD");

            Copy_Field(item, form, "colPD_TerminPlatnosciPodatku");
            Copy_Id(item, form, "selZadanie_PD");
            Copy_Field(item, "enumStatusZadania", form, "colPD_StatusZadania");
            BLL.Tools.Set_Text(form, "colPD_StatusWysylki", string.Empty);

        }

        public static void Update_PDS_Data(SPListItem item)
        {
            string KEY = Create_KEY(item);
            int formId = Get_KartaKontrolnaId(item, KEY);

            SPListItem form = Get_KartaKontrolnaById(item.Web, formId);
            Copy_PDFields(item, form);
            Copy_Field(item, form, "colIloscDokWBPKN");
            Copy_Field(item, form, "colIloscFaktur");
            Copy_Field(item, form, "colKosztyNKUP");
            Copy_Field(item, form, "colKosztyNKUP_WynWyl");
            Copy_Field(item, form, "colKosztyNKUP_ZUSPlatWyl");
            Copy_Field(item, form, "colKosztyNKUP_FakWyl");
            Copy_Field(item, form, "colKosztyNKUP_PozostaleKoszty");
            Copy_Field(item, form, "colKosztyWS");
            Copy_Field(item, form, "colKosztyWS_WynWlaczone");
            Copy_Field(item, form, "colKosztyWS_ZUSPlatWlaczone");
            Copy_Field(item, form, "colKosztyWS_FakWlaczone");
            Copy_Field(item, form, "colPrzychodyNP");
            Copy_Field(item, form, "colPrzychodyNP_DywidendySpO");
            Copy_Field(item, form, "colPrzychodyNP_Inne");
            Copy_Field(item, form, "colPrzychodyZwolnione");
            Copy_Field(item, form, "colWplaconaSZ");
            Copy_Field(item, form, "colZyskStrataNetto");

            Copy_Field(item, form, "colStrataDoOdliczenia");
            Copy_Field(item, form, "colStronaWn");
            Copy_Field(item, form, "colStronaMa");
            Copy_Field(item, form, "colStronaWn-StronaMa");
            Copy_Field(item, form, "colWplaconeZaliczkiOdPoczatkuRoku");
            Copy_Field(item, form, "colIleDoDoplaty");

            BLL.Models.Klient k = new Models.Klient(item.Web, Get_LookupId(item, "selKlient"));
            form["enumFormaPrawna"] = k.FormaPrawna;

            form.SystemUpdate();
        }

        public static void Update_VAT_Data(SPListItem item)
        {
            string KEY = Create_KEY(item);
            int formId = Get_KartaKontrolnaId(item, KEY);

            SPListItem form = Get_KartaKontrolnaById(item.Web, formId);
            Copy_Field(item, form, "colFormaOpodatkowaniaVAT");
            Copy_Field(item, form, "enumRozliczenieVAT");
            Copy_Field(item, form, "colVAT_WartoscNadwyzkiZaPoprzedn");
            Copy_Field(item, form, "colVAT_Decyzja");
            Copy_Field(item, form, "colVAT_WartoscDoZaplaty");
            Copy_Field(item, form, "colVAT_WartoscDoPrzeniesienia");
            Copy_Field(item, form, "colVAT_WartoscDoZwrotu");

            if (BLL.Tools.Get_Value(item, "colVAT_WartoscDoZwrotu") > 0)
            {
                Copy_Field(item, form, "colVAT_TerminZwrotuPodatku");
            }
            else
            {
                BLL.Tools.Set_Text(form, "colVAT_TerminZwrotuPodatku", string.Empty);
            }

            Copy_Field(item, form, "colVAT_eDeklaracja");
            Copy_Field(item, form, "colVAT_VAT-UE_Zalaczony");
            Copy_Field(item, form, "colVAT_VAT-27_Zalaczony");

            Copy_Field(item, "selOperator", form, "selOperator_VAT");
            Copy_Field(item, "colNotatka", form, "colUwagiVAT");

            Copy_Field(item, form, "colVAT_TerminPlatnosciPodatku");
            Copy_Id(item, form, "selZadanie_VAT");
            Copy_Field(item, "enumStatusZadania", form, "colVAT_StatusZadania");
            BLL.Tools.Set_Text(form, "colVAT_StatusWysylki", string.Empty);

            form.SystemUpdate();
        }

        public static void Update_ZUS_Data(SPListItem item)
        {
            string KEY = Create_KEY(item);
            int formId = Get_KartaKontrolnaId(item, KEY);

            SPListItem form = Get_KartaKontrolnaById(item.Web, formId);
            Copy_Field(item, form, "colFormaOpodakowania_ZUS");
            Copy_Field(item, form, "colZUS_SP_Skladka");
            Copy_Field(item, form, "colZUS_ZD_Skladka");
            Copy_Field(item, form, "colZUS_FP_Skladka");

            Copy_Field(item, form, "colZatrudniaPracownikow");
            if (BLL.Tools.Get_Flag(item, "colZatrudniaPracownikow"))
            {
                Copy_Field(item, form, "colZUS_PIT-4R_Zalaczony");
                Copy_Field(item, form, "colZUS_PIT-4R");
                Copy_Field(item, form, "colVAT_eDeklaracja");
                Copy_Field(item, form, "colZUS_PIT-8AR_Zalaczony");
                Copy_Field(item, form, "colZUS_PIT-8AR");
                Copy_Field(item, form, "colZUS_ListaPlac_Zalaczona");
                Copy_Field(item, form, "colZUS_Rachunki_Zalaczone");

                //ustaw termin płątności na podstawie miesięcznego terminu podatku dochodowego
                BLL.Tools.Set_Date(form, "colZUSPD_TerminPlatnosciPodatku", BLL.tabOkresy.Get_TerminPlatnosciByOkresId(item.Web, "colPD_TerminPlatnosciPodatku", BLL.Tools.Get_LookupId(item, "selOkres")));
            }

            Copy_Field(item, form, "colZUS_TerminPlatnosciSkladek");

            Copy_Field(item, "selOperator", form, "selOperator_ZUS");

            Copy_Field(item, "colNotatka", form, "colUwagiKadrowe");

            BLL.Models.Klient k = new Models.Klient(item.Web, Get_LookupId(item, "selKlient"));
            if (k.DataRozpoczeciaDzialalnosci!=new DateTime())
            {
                form["colDataRozpoczeciaDzialalnosci"] = k.DataRozpoczeciaDzialalnosci;
            }

            Copy_Field(item, "colNotatka", form, "colUwagiKadrowe");

            Copy_Id(item, form, "selZadanie_ZUS");
            Copy_Field(item, "enumStatusZadania", form, "colZUS_StatusZadania");
            BLL.Tools.Set_Text(form, "colZUS_StatusWysylki", string.Empty);

            form.SystemUpdate();
        }

        private static void Ensure(ref Models.Klient iok)
        {
            throw new NotImplementedException();
        }

        public static void Update_RBR_Data(SPListItem item)
        {
            string KEY = Create_KEY(item);
            int formId = Get_KartaKontrolnaId(item, KEY);

            SPListItem form = Get_KartaKontrolnaById(item.Web, formId);
            Copy_Field(item, form, "colBR_DataWystawieniaFaktury");
            Copy_Field(item, form, "colBR_NumerFaktury");
            Copy_Field(item, form, "colBR_WartoscDoZaplaty");
            Copy_Field(item, form, "colBR_TerminPlatnosci");
            Copy_Field(item, form, "colBR_FakturaZalaczona");

            Copy_Field(item, "selOperator", form, "selOperator_RBR");
            Copy_Field(item, "colNotatka", form, "colUwagiRBR");

            Copy_Id(item, form, "selZadanie_RBR");
            Copy_Field(item, "enumStatusZadania", form, "colRBR_StatusZadania");
            BLL.Tools.Set_Text(form, "colRBR_StatusWysylki", string.Empty);

            form.SystemUpdate();
        }

        public static void Update_PD_DataWysylki(SPListItem item, DateTime dateTime)
        {
            string KEY = Create_KEY(item);
            int formId = Get_KartaKontrolnaId(item, KEY);

            SPListItem form = Get_KartaKontrolnaById(item.Web, formId);
            form["colPD_DataWylaniaInformacji"] = dateTime;
            form.SystemUpdate();
        }

        public static void Update_VAT_DataWysylki(SPListItem item, DateTime dateTime)
        {
            string KEY = Create_KEY(item);
            int formId = Get_KartaKontrolnaId(item, KEY);

            SPListItem form = Get_KartaKontrolnaById(item.Web, formId);
            form["colVAT_DataWyslaniaInformacji"] = dateTime;
            form.SystemUpdate();
        }

        public static void Update_ZUS_DataWysylki(SPListItem item, DateTime dateTime)
        {
            string KEY = Create_KEY(item);
            int formId = Get_KartaKontrolnaId(item, KEY);

            SPListItem form = Get_KartaKontrolnaById(item.Web, formId);
            form["colZUS_DataWyslaniaInformacji"] = dateTime;
            form.SystemUpdate();
        }

        public static void Update_RBR_DataWysylki(SPListItem item, DateTime dateTime)
        {
            string KEY = Create_KEY(item);
            int formId = Get_KartaKontrolnaId(item, KEY);

            SPListItem form = Get_KartaKontrolnaById(item.Web, formId);
            form["colBR_DataPrzekazania"] = dateTime;
            form.SystemUpdate();
        }


        private static SPListItem Get_KartaKontrolnaById(SPWeb web, int formId)
        {
            SPList list = web.Lists.TryGetList(targetList);
            return list.Items.GetItemById(formId);
        }

        private static int Get_KartaKontrolnaId(SPListItem zadanieItem, string KEY)
        {
            SPList list = zadanieItem.Web.Lists.TryGetList(targetList);
            SPListItem item = list.Items.Cast<SPListItem>()
                .Where(i => BLL.Tools.Get_Text(i,"KEY").Equals(KEY))
                .FirstOrDefault();
            if (item != null)
            {
                return item.ID;
            }
            else
            {
                return Create_KartaKontrolna(zadanieItem, KEY, list);
            }

        }

        private static int Create_KartaKontrolna(SPListItem task, string KEY, SPList list)
        {
            SPListItem newItem = list.AddItem();
            newItem["KEY"] = KEY;
            newItem["selKlient"] = Get_LookupId(task, "selKlient");
            newItem["selOkres"] = Get_LookupId(task, "selOkres");

            BLL.Models.Klient k = new Models.Klient(task.Web, Get_LookupId(task, "selKlient"));

            Set_KartaKontrolna_InitValues(newItem, k);

            //ustaw CT
            if (k.TypKlienta == "KSH") newItem["ContentType"] = "Karta kontrolna KSH";
            else newItem["ContentType"] = "Karta kontrolna KPiR";

            newItem.SystemUpdate();

            return newItem.ID;
        }

        private static void Set_KartaKontrolna_InitValues(SPListItem newItem, BLL.Models.Klient k)
        {
            newItem["enumRozliczeniePD"] = k.RozliczeniePD;
            newItem["enumRozliczenieVAT"] = k.RozliczenieVAT;
            newItem["colFormaOpodatkowaniaPD"] = k.FormaOpodatkowaniaPD;
            newItem["colFormaOpodatkowaniaVAT"] = k.FormaOpodatkowaniaVAT;
            newItem["colFormaOpodakowania_ZUS"] = k.FormaOpodatkowaniaZUS;
            newItem["colVAT_TerminZwrotuPodatku"] = string.Empty;
            newItem["colZatrudniaPracownikow"] = k.ZatrudniaPracownikow;
        }

        private static int Get_KartaKontrolnaId(SPWeb web, int klientId, int okresId, string KEY, Models.Klient iok)
        {
            if (iok == null)
            {
                iok = new Models.Klient(web, klientId);
            }

            SPList list = web.Lists.TryGetList(targetList);
            SPListItem item = list.Items.Cast<SPListItem>()
                .Where(i => i["KEY"].ToString().Equals(KEY))
                .FirstOrDefault();
            if (item != null)
            {
                return item.ID;
            }
            else
            {
                return 0;
            }
        }

        #region Helpers
        private static string Create_KEY(Microsoft.SharePoint.SPListItem item)
        {
            int klientId = Get_LookupId(item, "selKlient");
            int okresId = Get_LookupId(item, "selOkres");
            return Create_KEY(klientId, okresId);
        }

        public static string Create_KEY(int klientId, int okresId)
        {
            return string.Format(@"{0}::{1}", klientId.ToString(), okresId.ToString());
        }

        private static string Get_LookupValue(Microsoft.SharePoint.SPListItem item, string col)
        {
            return item[col] != null ? new SPFieldLookupValue(item[col].ToString()).LookupValue : string.Empty;
        }

        private static int Get_LookupId(Microsoft.SharePoint.SPListItem item, string col)
        {
            return item[col] != null ? new SPFieldLookupValue(item[col].ToString()).LookupId : 0;
        }

        private static void Copy_Field(SPListItem item, string col0, SPListItem form, string col1)
        {
            form[col1] = item[col0];
        }

        private static void Copy_Field(SPListItem item, SPListItem form, string col)
        {
            form[col] = item[col];
        }

        private static void Copy_Field(SPListItem item, SPListItem form, string col, int mnoznik)
        {
            form[col] = double.Parse(item[col].ToString()) * mnoznik;
        }

        private static void Copy_Id(SPListItem item, SPListItem form, string col)
        {
            form[col] = item.ID;
        }

        #endregion

        internal static double Get_WartoscNadwyzkiDoPrzeniesienia(SPWeb web, int klientId, int okresId)
        {
            SPList list = web.Lists.TryGetList(targetList);

            string KEY = BLL.tabKartyKontrolne.Create_KEY(klientId, okresId);
            int kkId = Get_KartaKontrolnaId(web, klientId, okresId, KEY, null);

            if (kkId > 0)
            {
                SPListItem item = Get_KartaKontrolnaById(web, kkId);
                if (item != null)
                {
                    return BLL.Tools.Get_Value(item, "colVAT_WartoscDoPrzeniesienia");
                }
            }

            return 0;
        }

        public static SPListItem GetItemById(SPWeb web, int itemId)
        {
            return web.Lists.TryGetList(targetList).Items.GetItemById(itemId);
        }

        public static void Set_StatusWysylki(SPListItem item, string col, string value)
        {
            BLL.Tools.Set_Text(item, col, value);
        }

        public static void Set_StatusZadania(SPListItem item, int zadanieId, string col, string value, DateTime date)
        {
            BLL.Tools.Set_Text(item, col, value);

            //aktualizacja zdania
            SPListItem zadanie = BLL.tabZadania.GetItemById(item.Web, zadanieId);

            BLL.Tools.Set_Date(zadanie, col, date);

            BLL.Tools.Set_Text(zadanie, "enumStatusZadania", value);
            zadanie.SystemUpdate();
        }

        public static void Set_DataWyslania(SPListItem item, string col, DateTime data)
        {
            item[col] = data;
        }

        public static void Set_StatusZadania(SPListItem item, string col, string status)
        {
            item[col] = status;
        }

        /// <summary>
        /// Aktualizuje informacje o dostarczeniu dokumentów niezależnie od wybrania akcji
        /// </summary>
        /// <param name="zadanieItem"></param>
        public static void Update_POD(SPListItem zadanieItem)
        {

                string KEY = Create_KEY(zadanieItem);
                int formId = Get_KartaKontrolnaId(zadanieItem, KEY);

                SPListItem form = Get_KartaKontrolnaById(zadanieItem.Web, formId);
                bool pod = BLL.Tools.Get_Flag(zadanieItem, "colPotwierdzenieOdbioruDokumentow");
                if (BLL.Tools.Get_Flag(form, "colPotwierdzenieOdbioruDokumentow") != pod)
                {
                    Debug.WriteLine("wymagana aktualizacja flagi POD na karcie kontrolnej");
                    BLL.Tools.Set_Flag(form, "colPotwierdzenieOdbioruDokumentow", pod);
                    form.SystemUpdate();
                    Debug.WriteLine("flaga POD=" + pod.ToString());
                }
 

        }

        internal static SPListItem Get_KK_ByKlientId_ByOkresId(SPWeb web, int klientId, int okresId)
        {
            return web.Lists.TryGetList(targetList).Items.Cast<SPListItem>()
                .Where(i => BLL.Tools.Get_LookupId(i, "selKlient").Equals(klientId)
                            && BLL.Tools.Get_LookupId(i, "selOkres").Equals(okresId))
                .FirstOrDefault();
        }

        public static Array Get_ZwolnioneDoWysylki(SPWeb web)
        {
            //ustaw okres karencji
            int offset = int.Parse(BLL.admSetup.GetValue(web, "KK_IGNORE_UPDATES_MINUTES"));
            DateTime targetDate = DateTime.Now.AddMinutes(-1 * offset);

            return web.Lists.TryGetList(targetList).Items.Cast<SPListItem>()
                .Where(i => BLL.Tools.Get_Date(i, "Modified") < targetDate) //nie ruszaj modyfikowanych w ciągu istatnicj x minut
                .Where(i => BLL.Tools.Get_Text(i, "colZUS_StatusZadania").Equals(_STATUS_ZADANIA_ZWOLNIONE)
                            || BLL.Tools.Get_Text(i, "colPD_StatusZadania").Equals(_STATUS_ZADANIA_ZWOLNIONE)
                            || BLL.Tools.Get_Text(i, "colVAT_StatusZadania").Equals(_STATUS_ZADANIA_ZWOLNIONE)
                            || BLL.Tools.Get_Text(i, "colRBR_StatusZadania").Equals(_STATUS_ZADANIA_ZWOLNIONE))

                .ToArray();
        }
    }
}
