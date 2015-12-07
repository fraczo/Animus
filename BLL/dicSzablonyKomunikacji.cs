using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace BLL
{
    public class dicSzablonyKomunikacji
    {
        const string targetList = "Szablony komunikacji";

        public static void Get_TemplateByKod(SPListItem item, string kod, out string temat, out string trescHTML)
        {
            Get_TemplateByKod(item, kod, out temat, out trescHTML, string.Empty);
        }

        public static void Get_TemplateByKod(SPListItem item, string kod, out string temat, out string trescHTML, string nadawcaEmail)
        {
            switch (item.ParentList.Title)
            {
                case "Zadania":
                    //zobacz czy operator jest przypisany do zadania

                    string temp = string.Empty;
                    string footerTR = string.Empty;
                    Get_TemplateByKod(item.Web, "EMAIL_FOOTER_TR", out temp, out footerTR, false);

                    if (string.IsNullOrEmpty(nadawcaEmail))
                    {
                        int operatorId = Get_LookupId(item, "selOperator");
                        footerTR = Format_FooterTR(item, footerTR, operatorId);
                    }
                    else
                    {
                        int operatorId = BLL.dicOperatorzy.Get_OperatorIdByEmail(item.Web, nadawcaEmail);
                        footerTR = Format_FooterTR(item, footerTR, operatorId);
                    }

                    Get_TemplateByKod(item.Web, kod, out temat, out trescHTML, true);
                    trescHTML = trescHTML.Replace("___FOOTER___", footerTR);


                    break;

                default:
                    Get_TemplateByKod(item.Web, kod, out temat, out trescHTML, true);
                    break;
            }



        }

        private static string Format_FooterTR(SPListItem item, string footerTR, int operatorId)
        {
            if (operatorId > 0)
            {
                //użyj stopki konkretnego operatora
                BLL.Models.Operator op = new Models.Operator(item.Web, operatorId);

                footerTR = footerTR.Replace("___NAME___", op.Name);
                footerTR = footerTR.Replace("___CONTACT___", string.Format(@"{0}<br>{1}", op.Email, op.Telefon));
            }
            else
            {
                footerTR = string.Empty;
            }

            return footerTR;
        }

        /// <summary>
        /// pobiera odpowiedni szablon wiadomości i ukrywa sekcję footer jeżeli flaga nie jest ustawiona
        /// </summary>
        public static void Get_TemplateByKod(SPWeb web, string kod, out string temat, out string trescHTML, bool hasFooter)
        {
            SPList list = web.Lists.TryGetList(targetList);
            SPListItem item = list.Items.Cast<SPListItem>()
                .Where(i => i.Title == kod)
                .FirstOrDefault();

            temat = item["colTematWiadomosci"] != null ? item["colTematWiadomosci"].ToString() : string.Empty;
            trescHTML = item["colHTML"] != null ? item["colHTML"].ToString() : string.Empty;

            //zapakuj treść do szablonu
            if (kod.EndsWith(".Include"))
            {
                string sTemat = string.Empty;
                string sTrescHTML = string.Empty;
                Get_TemplateByKod(item, "EMAIL_DEFAULT_BODY", out sTemat, out sTrescHTML);
                StringBuilder sb = new StringBuilder(sTrescHTML);
                sb.Replace("___BODY___", trescHTML);

                //wyczyść stopkę jeżeli nie jest potrzebna
                if (!hasFooter) sb.Replace("___FOOTER___", string.Empty);

                trescHTML = sb.ToString();
            }
        }

        public static string Get_TemplateByKod(SPListItem item, string kod, bool hasFooter)
        {
            string temp;
            string trescHTML = string.Empty;
            Get_TemplateByKod(item, kod, out temp, out trescHTML);

            return trescHTML;
        }

        private static int Get_LookupId(SPListItem item, string col)
        {
            return item[col] != null ? new SPFieldLookupValue(item[col].ToString()).LookupId : 0;
        }





        public static string Get_TemplateByKod(SPWeb web, string kod, bool hasFooter)
        {
            string temp;
            string trescHTML = string.Empty;
            SPList list = web.Lists.TryGetList(targetList);
            SPListItem item = list.AddItem(); //tylko fikcyjnie tworzy rekod żeby mieć referencję ale go nie zapisuje
            Get_TemplateByKod(item, kod, out temp, out trescHTML);

            return trescHTML;
        }

        public static string Get_HTMLByKod(SPWeb web, string kod)
        {
            SPList list = GetList(web);
            SPListItem item = list.Items.Cast<SPListItem>()
                .Where(i => i.Title.Equals(kod))
                .FirstOrDefault();

            if (item != null) return BLL.Tools.Get_Text(item, "colHTML");
            else return string.Empty;
        }

        private static SPList GetList(SPWeb web)
        {
            return web.Lists.TryGetList(targetList);
        }

        public static string Ensure_HTMLByKod(SPWeb web, string kod)
        {
            SPList list = web.Lists.TryGetList(targetList);
            SPListItem item = list.Items.Cast<SPListItem>()
                .Where(i => i.Title.Equals(kod))
                .FirstOrDefault();

            if (item != null)
            {
                return BLL.Tools.Get_Text(item, "colHTML");
            }
            else
            {
                //dodaj rekord do listy
                SPListItem newItem = list.AddItem();
                BLL.Tools.Set_Text(newItem, "Title", kod);
                switch (kod)
                {
                    case "TABLE_TEMPLATE":
                        BLL.Tools.Set_Text(newItem, "colHTML", @"<table align=""center"" cellpadding=""5"" cellspacing=""2"" style=""width: 100%; font-family: Arial, Helvetica, sans-serif; font-size: x-small; text-align: center;"">[[ROWS]]</table>");
                        BLL.Tools.Set_Text(newItem, "colOpis", "[[ROWS]]");
                        break;
                    case "TR_TEMPLATE.Include":
                        BLL.Tools.Set_Text(newItem, "colHTML", @"<tr><th style=""height: 17px; text-align: left; background-color: #FFFFFF; width: 10%; font-family: Arial, Helvetica, sans-serif;"">&nbsp;</th> <th style=""height: 17px; text-align: left; background-color: #E4E4E4; width: 60%; font-family: Arial, Helvetica, sans-serif;""><span style=""font-weight: normal"">[[Opis]]</span></th> <td style=""height: 17px; background-color: #F4F4F4; width: 20%;"">[[Wartosc]]</td> <td style=""height: 17px; background-color: #FFFFFF; width: 10%;"">&nbsp;</td> </tr>");
                        BLL.Tools.Set_Text(newItem, "colOpis", @"");
                        break;
                    case "INFO_TEMPLATE":
                        BLL.Tools.Set_Text(newItem, "colHTML", @"<h3 style=""text-align: center; font-family: Arial, Helvetica, sans-serif; color: #808080;"">[[Tytul]]</h3>[[TABLE]] <blockquote><div style=""text-align: left; font-family: Arial, Helvetica, sans-serif; font-size: x-small;"">[[Tresc]]</div></blockquote>");
                        BLL.Tools.Set_Text(newItem, "colOpis", @"[[Tytul]], [[TABLE]], [[Tresc]]");
                        break;
                    default:
                        BLL.Tools.Set_Text(newItem, "colOpis", "To be completed...");
                        break;
                }

                newItem.SystemUpdate();

                return string.Empty;
            }
        }
    }
}
