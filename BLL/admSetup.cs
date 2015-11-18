using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using BLL.Models;

namespace BLL
{
    public class admSetup
    {
        const string targetList = @"admSetup";

        internal static Models.KontaZUS GetKontaZUS(Microsoft.SharePoint.SPWeb web)
        {
            SPList list  = web.Lists.TryGetList(targetList);

            KontaZUS obj = new Models.KontaZUS();

            if (list != null)
            {
                //SP
                SPListItem item = list.Items.Cast<SPListItem>()
                    .Where(i => i["KEY"].ToString() == @"ZUS_SP_KONTO")
                    .FirstOrDefault();

                if (item!=null)
                {
                    obj.KontoSP = item["VALUE"].ToString();
                }

                //ZD
                item = list.Items.Cast<SPListItem>()
                    .Where(i => i["KEY"].ToString() == @"ZUS_ZD_KONTO")
                    .FirstOrDefault();

                if (item != null)
                {
                    obj.KontoZD = item["VALUE"].ToString();
                }

                //FP
                item = list.Items.Cast<SPListItem>()
                    .Where(i => i["KEY"].ToString() == @"ZUS_FP_KONTO")
                    .FirstOrDefault();

                if (item != null)
                {
                    obj.KontoFP = item["VALUE"].ToString();
                }
            }

            return obj;

        }

        public static string GetValue(SPWeb web, string key)
        {
            SPList list = web.Lists.TryGetList(targetList);
            //if (list!=null)
            //{
                SPListItem item = list.Items.Cast<SPListItem>()
                    .Where(i => i["KEY"].ToString() == key)
                    .FirstOrDefault();

                if (item!=null)
                {
                    return item["VALUE"].ToString();
                }
            //}

            return string.Empty;
        }

        public static string GetText(SPWeb web, string key)
        {
            SPList list = web.Lists.TryGetList(targetList);
            //if (list != null)
            //{
                SPListItem item = list.Items.Cast<SPListItem>()
                    .Where(i => i["KEY"].ToString() == key)
                    .FirstOrDefault();

                if (item != null)
                {
                    return item["TEXT"].ToString();
                }
            //}

            return string.Empty;
        }

        public static string Get_NazwaBiura(SPWeb web)
        {
            string result = GetValue(web, "BR_NAZWA");
            string v = GetValue(web, "BR_ADRES");
            if (!string.IsNullOrEmpty(v)) result = result + " " + v;
            v = GetValue(web, "BR_KOD_POCZTOWY");
            if (!string.IsNullOrEmpty(v)) result = result + " " + v;
            v = GetValue(web, "BR_MIEJSCOWOSC");
            if (!string.IsNullOrEmpty(v)) result = result + " " + v;

            return result;
        }

        public static bool IsProductionEnabled(SPWeb web)
        {

            string proKEY = "PRODUCTION_MODE";
            string proEnabled = "Enabled";
            string proDisabled = "Disabled";

            string v = GetValue(web, proKEY);
            if (v == proEnabled)
            {
                return true;
            }
            else
            {
                //dodaj nieaktywny klucz
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    BLL.admSetup.Ensure(web, proKEY, proDisabled, "VALUE", "Przełącznik odblokowujący produkcyjną pracę systemu");
                });

                return false;
            }

        }

        private static void Ensure(SPWeb web, string key, string defaultValue, string columnName, string description)
        {
            SPList list = web.Lists.TryGetList(targetList);
            SPListItem item = list.Items.Cast<SPListItem>()
                .Where(i => i["KEY"].ToString() == key)
                .FirstOrDefault();

            if (item == null)
            {
                try
                {
                    item = list.AddItem();
                    item["KEY"] = key;
                    item[columnName] = defaultValue;
                    item["colOpis"] = description;
                    item.SystemUpdate();
                }
                catch (Exception ex)
                {
                    var result = ElasticEmail.EmailGenerator.ReportError(ex, web.Url);
                }

            }
        }
    }
}
