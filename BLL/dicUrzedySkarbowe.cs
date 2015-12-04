using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Diagnostics;

namespace BLL
{
    public class dicUrzedySkarbowe
    {
        const string targetList = "Urzędy skarbowe"; //"dicUrzedySkarbowe";

        internal static string Get_KontoPIT(SPWeb web, int urzadId)
        {
            string result = string.Empty;

            SPList list = web.Lists.TryGetList(targetList);

            SPListItem item = list.GetItemById(urzadId);
            if (item != null)
            {
                result = item["colPIT_Konto"] != null ? item["colPIT_Konto"].ToString() : string.Empty;
            }

            return result;
        }

        internal static string Get_KontoVAT(SPWeb web, int urzadId)
        {
            string result = string.Empty;

            SPList list = web.Lists.TryGetList(targetList);

            SPListItem item = list.GetItemById(urzadId);
            if (item != null)
            {
                result = item["colVAT_Konto"] != null ? item["colVAT_Konto"].ToString() : string.Empty; ;
            }

            return result;
        }

        internal static string Get_KontoCIT(SPWeb web, int urzadId)
        {
            string result = string.Empty;

            SPList list = web.Lists.TryGetList(targetList);
            //if (list != null)
            //{
            SPListItem item = list.GetItemById(urzadId);
            if (item != null)
            {
                result = item["colCIT_Konto"] != null ? item["colCIT_Konto"].ToString() : string.Empty;
            }
            //}

            return result;
        }


        internal static int Ensure(SPWeb web, int urzadId)
        {
            if (urzadId > 0)
            {
                SPList list = web.Lists.TryGetList(targetList);
                try
                {
                    SPListItem item = list.GetItemById(urzadId);
                    if (item != null)
                    {
                        return item.ID;
                    }
                }
                catch (Exception)
                { }
            }

            return 0;
        }

        public static bool Check_IfExist(SPWeb web, int urzadId)
        {
            Debug.WriteLine("BLL.dicUrzedySkarbowe.Check_IfExist");

            Debug.WriteLine("urzadId=" + urzadId.ToString());

            if (urzadId > 0)
            {
                SPList list = GetList(web);
                SPListItem item = null;
                try
                {
                    item = list.GetItemById(urzadId);

                    Debug.WriteLine("urzadId=" + urzadId.ToString() + " znaleziony");
                }
                catch (Exception)
                {
                    Debug.WriteLine("urzadId=" + urzadId.ToString() + " nie znaleziony");
                }

                if (item != null) return true;
                else return false;
            }

            return false;
        }

        private static SPList GetList(SPWeb web)
        {
            return web.Lists.TryGetList(targetList);
        }
    }
}
