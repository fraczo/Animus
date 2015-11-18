using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace EventReceivers.admProcesy
{
    class GFR_Request
    {
        internal static void Create(Microsoft.SharePoint.SPListItem item)
        {
            string mask = BLL.Tools.Get_Text(item, "colMaskaSerwisu");
            string kmask = BLL.Tools.Get_Text(item, "colMaskaTypuKlienta");

            if (!string.IsNullOrEmpty(kmask))
            {
                if (!string.IsNullOrEmpty(mask))
                {
                    Create_Bulk_FormsBy_KMask_Mask(item, kmask, mask);
                }
                else
                {
                    Crate_Bulk_FormsBy_KMask(item, kmask);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(mask))
                {
                    Create_Bulk_FormsBy_Mask(item, mask);
                }
                else
                {
                    Crate_Bulk_Forms(item);
                }
            }
        }

        private static void Create_Bulk_FormsBy_KMask_Mask(SPListItem item, string kmask, string mask)
        {
            Array klienci = BLL.tabKlienci.Get_AktywniKlienci_ByTypKlienta_BySerwisMask(item.Web, kmask, mask);
            Create_Forms(item, klienci);
        }

        private static void Crate_Bulk_FormsBy_KMask(SPListItem item, string kmask)
        {
            Array klienci = BLL.tabKlienci.Get_AktywniKlienci_ByTypKlientaMask(item.Web, kmask);
            Create_Forms(item, klienci);
        }

        private static void Create_Bulk_FormsBy_Mask(SPListItem item, string mask)
        {
            Array klienci = BLL.tabKlienci.Get_AktywniKlienci_BySerwisMask(item.Web, mask);
            Create_Forms(item, klienci);
        }

        private static void Crate_Bulk_Forms(SPListItem item)
        {
            Array klienci = BLL.tabKlienci.Get_AktywniKlienci_Serwis(item.Web);
            Create_Forms(item, klienci);
        }

        private static void Create_Forms(SPListItem item, Array klienci)
        {
            SPList list = BLL.admProcesy.GetList(item.Web);

            string mask = BLL.Tools.Get_Text(item, "colMaskaSerwisu");

            foreach (SPListItem k in klienci)
            {
                if (string.IsNullOrEmpty(mask))
                {
                    Create_New_GFR_K(item, "ZUS-*", list, k);
                    Create_New_GFR_K(item, "PD-*", list, k);
                    Create_New_GFR_K(item, "PDS-*", list, k);
                    Create_New_GFR_K(item, "VAT-*", list, k);
                    //Create_New_GFR_K(item, "RBR", list, k);
                }
                else
                {
                    Create_New_GFR_K(item, mask, list, k);
                }

            }
        }

        private static void Create_New_GFR_K(Microsoft.SharePoint.SPListItem item, string mask, SPList list, SPListItem klientItem)
        {

                string ct = "Generowanie formatek rozliczeniowych dla klienta";
                int okresId = BLL.Tools.Get_LookupId(item, "selOkres");

                SPListItem newItem = list.AddItem();
                newItem["ContentType"] = ct;
                newItem["selKlient"] = klientItem.ID;
                newItem["selOkres"] = okresId;
                newItem["colMaskaSerwisu"] = mask;

                newItem.SystemUpdate();
        }
    }
}
