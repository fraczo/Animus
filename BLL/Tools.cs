using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using BLL.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;

namespace BLL
{
    public class Tools
    {
        const string emptyMarker = "---";

        internal static string Get_ItemInfo(Microsoft.SharePoint.SPListItem item)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CT=" + item.ContentType.Name);
            sb.AppendLine("ID=" + item.ID.ToString());

            return sb.ToString();
        }

        public static void Ensure_LinkColumn(SPListItem item, string sourceColumn)
        {
            string col1 = sourceColumn + "_LINK";
            string col2 = sourceColumn + "_LINKID";

            Ensure_Column(item, col1);
            item[col1] = item[sourceColumn] != null ? item[sourceColumn].ToString() : string.Empty;

            Ensure_Column(item, col2);
            item[col2] = item[sourceColumn] != null ? new SPFieldLookupValue(item[sourceColumn].ToString()).LookupId.ToString() : string.Empty;

            item.SystemUpdate();

        }

        /// <summary>
        /// definiuje kolumnę w razie potrzeby
        /// </summary>
        private static void Ensure_Column(SPListItem item, string targetColumn)
        {
            bool found = false;
            SPList list = item.ParentList;
            foreach (SPField col in list.Fields)
            {
                if (col.InternalName == targetColumn)
                {
                    found = true;
                    break;
                }
            }

            if (!found) Create_Column(list, targetColumn);
        }

        private static void Create_Column(SPList list, string targetColumn)
        {
            SPFieldText f = (SPFieldText)list.Fields.CreateNewField(SPFieldType.Text.ToString(), targetColumn);

            list.Fields.Add(f);
            list.Update();
        }


        public static string AddCompanyName(SPWeb web, string temat, int klientId)
        {
            if (klientId > 0)
            {
                BLL.Models.Klient k = new Klient(web, klientId);
                return string.Format("{0} {1}", temat, k.PelnaNazwaFirmy);
            }

            return temat;
        }

        public static string AddCompanyName(string temat, SPListItem item)
        {
            if (item != null)
            {
                if (item.ContentType.Name == "KPiR" || item.ContentType.Name == "KSH")
                {
                    int klientId = Get_LookupId(item, "selKlient");
                    if (klientId > 0)
                    {
                        BLL.Models.Klient k = new Klient(item.Web, klientId);
                        return string.Format("{0} {1}", temat, k.PelnaNazwaFirmy);
                    }
                }

                if (item.ContentType.Name == "Prośba o dokumenty"
                    || item.ContentType.Name == "Prośba o przesłanie wyciągu bankowego"
                    || item.ContentType.Name == "Rozliczenie podatku dochodowego"
                    || item.ContentType.Name == "Rozliczenie podatku dochodowego spółki"
                    || item.ContentType.Name == "Rozliczenie podatku VAT"
                    || item.ContentType.Name == "Rozliczenie z biurem rachunkowym"
                    || item.ContentType.Name == "Rozliczenie ZUS")
                {
                    int klientId = Get_LookupId(item, "selKlient");
                    if (klientId > 0)
                    {
                        BLL.Models.Klient k = new Klient(item.Web, klientId);
                        return string.Format("{0} {1}", temat, k.PelnaNazwaFirmy);
                    }
                }
            }
            return temat;
        }

        public static int Get_LookupId(SPListItem item, string col)
        {
            return item[col] != null ? new SPFieldLookupValue(item[col].ToString()).LookupId : 0;
        }

        public static string Format_Currency(SPListItem item, string colName)
        {
            double n = Get_Value(item, colName);

            if (n > 0) return n.ToString("c", new CultureInfo("pl-PL"));
            else return emptyMarker;

        }

        public static double Get_Value(SPListItem item, string colName)
        {
            if (item[colName] != null)
            {
                return double.Parse(item[colName].ToString());
            }
            else
            {
                return 0;
            }
        }

        public static string Format_Currency(double value)
        {
            if (value > 0) return value.ToString("c", new CultureInfo("pl-PL"));
            else return emptyMarker;
        }

        public static DateTime Get_Date(SPListItem item, string col)
        {
            return item[col] != null ? DateTime.Parse(item[col].ToString()) : new DateTime();
        }

        public static string Get_Text(SPListItem item, string col)
        {
            return item[col] != null ? item[col].ToString() : string.Empty;
        }

        public static bool Get_Flag(SPListItem item, string col)
        {
            return item[col] != null ? bool.Parse(item[col].ToString()) : false;
        }


        // zakłada format wejściowy YYYY-MM
        public static string Get_KwartalDisplayName(string okres)
        {
            if (okres.Length == 7)
            {
                string rok = okres.Substring(0, 4);
                string miesiac = okres.Substring(5, 2);
                int mNumber = int.Parse(miesiac);
                switch (mNumber)
                {
                    case 1:
                    case 2:
                    case 3:
                        return rok + "-K01";
                    case 4:
                    case 5:
                    case 6:
                        return rok + "-K02";
                    case 7:
                    case 8:
                    case 9:
                        return rok + "-K03";
                    case 10:
                    case 11:
                    case 12:
                        return rok + "-K04";
                    default:
                        return string.Empty;
                }
            }
            return string.Empty;
        }

        public static string Get_LookupValue(SPListItem item, string col)
        {
            return item[col] != null ? new SPFieldLookupValue(item[col].ToString()).LookupValue : string.Empty;
        }

        internal static string Get_UserEmail(SPListItem item, string col)
        {
            string result = item[col] != null ? new SPFieldUserValue(item.Web, item[col].ToString()).User.Email : string.Empty;
            if (BLL.Tools.Is_ValidEmail(result)) return result;
            else return string.Empty;
        }

        internal static bool Is_ValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        internal static string Get_CurrentUser(SPListItem item)
        {
            string result = item["Editor"] != null ? new SPFieldUserValue(item.Web, item["Editor"].ToString()).User.Email : string.Empty;

            if (string.IsNullOrEmpty(result))
            {
                //ustaw domyślnie adres biura
                result = BLL.admSetup.GetValue(item.Web, "EMAIL_BIURA");
            }

            if (BLL.Tools.Is_ValidEmail(result))
            {
                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string Cleanup_Text(SPListItem item, string col)
        {
            string s = item[col] != null ? item[col].ToString() : string.Empty;
            string s0;
            do
            {
                s0 = s;
                s = Regex.Replace(s.Trim(), @"\s\s", " ");
            } while (!s.Equals(s0));

            return s;
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);

                if (addr.Address == email)
                {
                    //Additional validation

                    string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                    @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                    @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

                    Regex re = new Regex(strRegex);

                    if (re.IsMatch(email)) return true;
                    else return (false);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static string AppendNote_Top(SPListItem item, string col, string memo, bool forceTimeStamp)
        {
            StringBuilder sb = new StringBuilder(@"#" + memo);

            if (forceTimeStamp)
            {
                sb = new StringBuilder(@"#" + DateTime.Now.ToString());
                sb.AppendLine(memo);
            }

            sb.AppendLine(item[col] != null ? item[col].ToString() : string.Empty);

            return sb.ToString();
        }

        internal static Array Get_LookupValueCollection(SPListItem item, string col)
        {
            return item[col] != null ? new SPFieldLookupValueCollection(item[col].ToString()).ToArray() : null;
        }

        public static void Set_Text(SPListItem item, string col, string val, bool updateRequest)
        {
            if (item[col] != null && item[col].ToString() != val.ToString())
            {
                item[col] = val.ToString();

                if (updateRequest)
                {
                    item.SystemUpdate();
                }
            }

        }

        public static bool IsSelectorAssigned(SPListItem item, string col, string exceptValue)
        {
            if (item[col] == null) return false;

            string v = item[col].ToString();
            if (string.IsNullOrEmpty(v)) return false;
            if (v.Equals(exceptValue)) return false;

            return true;
        }

        public static void Remove_Services(ref SPListItem item, string col, string mask)
        {
            SPFieldLookupValueCollection newOptions = new SPFieldLookupValueCollection();
            SPFieldLookupValueCollection options = new SPFieldLookupValueCollection(item[col].ToString());
            foreach (SPFieldLookupValue option in options)
            {
                if (mask.EndsWith("*"))
                {
                    mask = mask.Substring(0, mask.Length - 1);
                    if (!option.LookupValue.StartsWith(mask))
                    {
                        newOptions.Add(option);
                    }
                }
                else
                {
                    if (!option.LookupValue.Equals(mask))
                    {
                        newOptions.Add(option);
                    }
                }
            }

            item[col] = newOptions;
        }

        public static void Assign_Service(ref SPListItem item, string col, string serwisName)
        {
            int serwisId = BLL.dicSerwisy.Get_IdByKod(item.Web, serwisName);
            if (serwisId > 0)
            {
                SPFieldLookupValue option = new SPFieldLookupValue(serwisId, serwisName);
                SPFieldLookupValueCollection options = new SPFieldLookupValueCollection(item[col].ToString());
                options.Add(option);
                item[col] = options;
            }
        }

        internal static double Get_Double(SPListItem item, string col)
        {
            return item[col] != null ? double.Parse(item[col].ToString()) : 0;
        }

        public static bool Has_SerwisAssigned(SPListItem item, string col, string mask)
        {
            Array aSerwisy = BLL.Tools.Get_LookupValueCollection(item, col);
            if (aSerwisy.Length > 0)
                if (mask.EndsWith("*"))
                {
                    mask = mask.Substring(0, mask.Length - 1);
                    foreach (SPFieldLookupValue s in aSerwisy)
                    {
                        if (s.LookupValue.StartsWith(mask))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    foreach (SPFieldLookupValue s in aSerwisy)
                    {
                        if (s.LookupValue.Equals(mask))
                        {
                            return true;
                        }
                    }
                }
            else
            {
                return false;
            }

            return false;
        }

        internal static int Get_Integer(SPListItem item, string col)
        {
            return item[col] != null ? Convert.ToInt32(item.ToString()) : 0;
        }

        internal static void Copy_Attachement(SPListItem item, SPFile file)
        {
            int bufferSize = 20480;
            byte[] byteBuffer = new byte[bufferSize];
            byteBuffer = file.OpenBinary();
            item.Attachments.Add(file.Name, byteBuffer);
        }

        public static void Set_Flag(ref SPListItem item, string col, bool value)
        {
            if (item[col]!=null)
            {
                item[col] = value;
            }
        }

        public static void Set_Value(ref SPListItem item, string col, double value)
        {
            if (item[col] != null)
            {
                item[col] = value;
            }
        }
    }
}
