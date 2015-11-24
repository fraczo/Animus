using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace BLL.Models
{
    class Okres
    {
        const string targetList = "Okresy";
        private Microsoft.SharePoint.SPWeb web;
        private int okresId;

        public Okres(Microsoft.SharePoint.SPWeb web, int okresId)
        {
            this.web = web;
            this.okresId = okresId;

            SPList list = web.Lists.TryGetList(targetList);
            SPListItem item = list.GetItemById(okresId);
            if (item != null)
            {
                this.Nazwa = BLL.Tools.Get_Text(item, "Title");
                this.DataRozpoczecia = BLL.Tools.Get_Date(item, "colDataRozpoczecia");
                this.DataZakonczenia = BLL.Tools.Get_Date(item, "colDataZakonczenia");

                Skladka_ZUS_M_SP = BLL.Tools.Get_Double(item, "colZUS_M_SP_Skladka");
                Skladka_ZUS_M_SPC = BLL.Tools.Get_Double(item, "colZUS_M_SPC_Skladka");
                Skladka_ZUS_D_SP = BLL.Tools.Get_Double(item, "colZUS_D_SP_Skladka");
                Skladka_ZUS_D_SPC = BLL.Tools.Get_Double(item, "colZUS_D_SPC_Skladka");
                Skladka_ZUS_M_ZD = BLL.Tools.Get_Double(item, "colZUS_M_ZD_Skladka");
                Skladka_ZUS_D_ZD = BLL.Tools.Get_Double(item, "colZUS_D_ZD_Skladka");
                Skladka_ZUS_M_FP = BLL.Tools.Get_Double(item, "colZUS_M_FP_Skladka");
                Skladka_ZUS_D_FP = BLL.Tools.Get_Double(item, "colZUS_D_FP_Skladka");

                TerminPlatnosciSkladek_ZUS_BezPracownikow = BLL.Tools.Get_Date(item, "colZUS_TerminPlatnosciSkladek_BezPracownikow");
                TerminPlatnosciSkladek_ZUS_ZPracownikami = BLL.Tools.Get_Date(item, "colZUS_TerminPlatnosciSkladek_ZPracownikami");
                TerminPrzekazaniaWynikow_ZUS_Ofset = BLL.Tools.Get_Double(item, "ZUS_TerminPrzekazaniaWynikow_Ofset");

                TerminPlatnosciPodatkuPD = BLL.Tools.Get_Date(item, "colPD_TerminPlatnosciPodatku");
                TerminPlatnosciPodatkuPD_KW = BLL.Tools.Get_Date(item, "colPD_TerminPlatnosciPodatkuKW");
                TerminPrzekazaniaWynikowPD_Ofset = BLL.Tools.Get_Double(item, "colPD_TerminPrzekazaniaWynikow_Ofset");

                TerminPlatnosciPodatkuVAT = BLL.Tools.Get_Date(item, "colVAT_TerminPlatnosciPodatku");
                TerminPlatnosciPodatkuVAT_KW = BLL.Tools.Get_Date(item, "colVAT_TerminPlatnosciPodatkuKW");
                TerminPrzekazaniaWynikowVAT_Ofset = BLL.Tools.Get_Double(item, "colVAT_TerminPrzekazaniaWynikow_Ofset");

                TerminPrzekazaniaRBR = BLL.Tools.Get_Date(item, "colBR_TerminPrzekazania");
            }
        }

        public object Skladka_ZUS_M_SP { get; set; }
        public object Skladka_ZUS_M_SPC { get; set; }
        public object Skladka_ZUS_D_SP { get; set; }
        public object Skladka_ZUS_D_SPC { get; set; }
        public object Skladka_ZUS_M_ZD { get; set; }
        public object Skladka_ZUS_D_ZD { get; set; }
        public object Skladka_ZUS_M_FP { get; set; }
        public object Skladka_ZUS_D_FP { get; set; }


        public DateTime TerminPlatnosciSkladek_ZUS_BezPracownikow { get; set; }

        public DateTime TerminPlatnosciSkladek_ZUS_ZPracownikami { get; set; }

        public double TerminPrzekazaniaWynikow_ZUS_Ofset { get; set; }

        public DateTime TerminPlatnosciPodatkuPD { get; set; }

        public DateTime TerminPlatnosciPodatkuPD_KW { get; set; }

        public double TerminPrzekazaniaWynikowPD_Ofset { get; set; }

        public DateTime TerminPlatnosciPodatkuVAT { get; set; }

        public DateTime TerminPlatnosciPodatkuVAT_KW { get; set; }

        public double TerminPrzekazaniaWynikowVAT_Ofset { get; set; }

        public DateTime TerminPrzekazaniaRBR { get; set; }

        public DateTime DataRozpoczecia { get; set; }
        public DateTime DataZakonczenia { get; set; }

        public string Nazwa { get; set; }
    }
}
