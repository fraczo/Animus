using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Text;

namespace EventReceivers.tabZadania
{
    public class tabZadania : SPItemEventReceiver
    {
        public override void ItemAdded(SPItemEventProperties properties)
        {
            Execute(properties);
        }

        public override void ItemUpdated(SPItemEventProperties properties)
        {
            Execute(properties);
        }

        private void Execute(SPItemEventProperties properties)
        {
            this.EventFiringEnabled = false;

            SPListItem item = properties.ListItem;

            BLL.Logger.LogEvent_EventReceiverInitiated(item);

            Set_Status(item);

            try
            {
                switch (item.ContentType.Name)
                {

                    case "Rozliczenie ZUS":
                        Manage_ZUS(item);
                        break;
                    case "Rozliczenie podatku dochodowego":
                        Manage_PD(item);
                        break;
                    case "Rozliczenie podatku dochodowego spółki":
                        Manage_PDS(item);
                        break;
                    case "Rozliczenie podatku VAT":
                        Manage_VAT(item);
                        break;
                    case "Rozliczenie z biurem rachunkowym":
                        Manage_RBR(item);
                        break;

                    case "Zadanie":
                        Manage_Zadanie(item);
                        break;

                    case "Wiadomość z ręki":
                    case "Wiadomość z szablonu":
                    case "Wiadomość grupowa":
                    case "Wiadomość grupowa z szablonu":
                        BLL.tabWiadomosci.CreateMailMessage(item);
                        break;

                    case "Informacja uzupełniająca":
                        //BLL.tabWiadomosci.CreateMailMessage_InformacjaUzupelniajaca(item);
                        break;

                    default:
                        new Exception("Nieobsługiwany typ zadania " + item.ContentType.Name);
                        break;
                }

                Update_Extras(item);

                item.SystemUpdate();
            }
            catch (Exception ex)
            {
                BLL.Tools.Set_Text(item, "enumStatusZadania", "Anulowane", true);
                BLL.Logger.LogEvent(properties.WebUrl, ex.ToString());
                var result = ElasticEmail.EmailGenerator.ReportError(ex, properties.WebUrl.ToString());
            }
            finally
            {
                BLL.Logger.LogEvent_EventReceiverCompleted(item);
                this.EventFiringEnabled = true;
            }
        }

        #region ZUS
        private void Manage_ZUS(SPListItem item)
        {
            if (IsValid_ZUS_Form(item))
            {
                if (IsValid_ZUS_MessageDetails(item))
                {

                }
            }
            else
            {

            }
        }

        private bool IsValid_ZUS_Form(SPListItem item)
        {
            bool result = true;
            StringBuilder errLog = new StringBuilder();

            //Składki ZUS
            bool zp = BLL.Tools.Get_Flag(item, "colZatrudniaPracownikow");
            string fo = BLL.Tools.Get_Text(item, "colFormaOpodakowania_ZUS");
            switch (fo)
            {
                case "Tylko zdrowotna":
                    if (true)
                    {
                        if (zp)
                        {
                            result = HasValue(item, "colZUS_SP_Skladka")
                                && HasValue(item, "colZUS_ZD_Skladka")
                                && HasValue(item, "colZUS_FP_Skladka");
                            if (!result) errLog.AppendLine("Nieprawidłowa warotść składki");
                        }
                        else
                        {
                            result = HasValue(item, "colZUS_ZD_Skladka");
                            if (!result) errLog.AppendLine("Nieprawidłowa warotść składki zdrowotnej");
                            BLL.Tools.Set_Value(ref item, "colZUS_SP_Skladka", 0);
                            BLL.Tools.Set_Value(ref item, "colZUS_FP_Skladka", 0);
                        }
                    }
                    break;

                case "Tylko pracownicy":
                    if (!zp)
                    {
                        zp = true;
                        BLL.Tools.Set_Flag(ref item, "colZatrudniaPracownikow", zp);
                    }

                    result = HasValue(item, "colZUS_SP_Skladka")
                        && HasValue(item, "colZUS_ZD_Skladka")
                        && HasValue(item, "colZUS_FP_Skladka");
                    if (!result) errLog.AppendLine("Nieprawidłowa warotść składki");
                   
                    break;

                default:
                    result = HasValue(item, "colZUS_SP_Skladka")
                        && HasValue(item, "colZUS_ZD_Skladka")
                        && HasValue(item, "colZUS_FP_Skladka");
                    if (!result) errLog.AppendLine("Nieprawidłowa warotść składki");
                    break;
            }

            //PIT-4R
            if (result && zp)
            {
                bool pit4R = BLL.Tools.Get_Flag(item, "colZUS_PIT-4R_Zalaczony");
                if (pit4R)
                {
                    result = HasValue(item, "colZUS_PIT-4R");
                    if (!result) errLog.AppendLine("Nieprawidłowa warotść PIT-4R");
                }
                else BLL.Tools.Set_Value(ref item, "colZUS_PIT-4R", 0);
            }

            //PIT-8AR
            if (result && zp)
            {
                bool pit8AR = BLL.Tools.Get_Flag(item, "colZUS_PIT-8AR_Zalaczony");
                if (pit8AR)
                {
                    result = HasValue(item, "colZUS_PIT-8AR");
                    if (!result) errLog.AppendLine("Nieprawidłowa warotść PIT-8AR");
                }
                else BLL.Tools.Set_Value(ref item, "colZUS_PIT-8AR", 0);
            }

            if (!zp)
            {
                BLL.Tools.Set_Flag(ref item, "colZUS_PIT-4R_Zalaczony", false);
                BLL.Tools.Set_Value(ref item, "colZUS_PIT-4R", 0);
                BLL.Tools.Set_Flag(ref item, "colZUS_PIT-8AR_Zalaczony", false);
                BLL.Tools.Set_Value(ref item, "colZUS_PIT-8AR", 0);
            }

            
            //Załączniki
            bool lpz = BLL.Tools.Get_Flag(item, "colZUS_ListaPlac_Zalaczona");
            bool rz = BLL.Tools.Get_Flag(item, "colZUS_Rachunki_Zalaczone");

            if (result && lpz && rz)
            {
                result = (item.Attachments.Count >= 2);
                if (!result) errLog.AppendLine("Brak załączników");
            }
            else if (result && (lpz || rz))
            {
                result = (item.Attachments.Count >= 1);
                if (!result) errLog.AppendLine("Brak załączników");
            }

            if (!result)
            {
                //ustaw flagę walidacji
                BLL.Tools.AppendNote_Top(item, "colNotatka", errLog.ToString(), true);
                BLL.Tools.Set_Flag(ref item, "_Validation", true);
                item.SystemUpdate();
            }
            else
            {
                //wyczyść flagę walidacji jeżeli ustawiona
                if (BLL.Tools.Get_Flag(item, "_Validation"))
                {
                    BLL.Tools.Set_Flag(ref item, "_Validation", false);
                    item.SystemUpdate();
                }
            }

            return result;
        }



        private bool IsValid_ZUS_MessageDetails(SPListItem item)
        {
            //todo: throw new NotImplementedException();
            return true;
        }
        #endregion

        #region PD
        private void Manage_PD(SPListItem item)
        {
            if (IsValid_PD_Form(item))
            {
                if (IsValid_PD_MessageDetails(item))
                {

                }
            }
            else
            {

            }
        }

        private bool IsValid_PD_Form(SPListItem item)
        {
            //todo: throw new NotImplementedException();
            return true;
        }

        private bool IsValid_PD_MessageDetails(SPListItem item)
        {
            //todo: throw new NotImplementedException();
            return true;
        }
        #endregion

        #region PDS
        private void Manage_PDS(SPListItem item)
        {
            if (IsValid_PDS_Form(item))
            {
                if (IsValid_PDS_MessageDetails(item))
                {

                }
            }
            else
            {

            }
        }

        private bool IsValid_PDS_Form(SPListItem item)
        {
            //todo: throw new NotImplementedException();
            return true;
        }

        private bool IsValid_PDS_MessageDetails(SPListItem item)
        {
            //todo: throw new NotImplementedException();
            return true;
        }
        #endregion

        #region VAT
        private void Manage_VAT(SPListItem item)
        {
            if (IsValid_VAT_Form(item))
            {
                if (IsValid_VAT_MessageDetails(item))
                {

                }
            }
            else
            {

            }
        }

        private bool IsValid_VAT_Form(SPListItem item)
        {
            //todo: throw new NotImplementedException();
            return true;
        }

        private bool IsValid_VAT_MessageDetails(SPListItem item)
        {
            //todo: throw new NotImplementedException();
            return true;
        }
        #endregion

        #region RBR
        private void Manage_RBR(SPListItem item)
        {
            if (IsValid_RBR_Form(item))
            {
                if (IsValid_RBR_MessageDetails(item))
                {

                }
            }
            else
            {

            }
        }

        private bool IsValid_RBR_Form(SPListItem item)
        {
            //todo: throw new NotImplementedException();
            return true;
        }

        private bool IsValid_RBR_MessageDetails(SPListItem item)
        {
            //todo: throw new NotImplementedException();
            return true;
        }
        #endregion

        private void Manage_Zadanie(SPListItem item)
        {
            //todo: throw new NotImplementedException();
        }

        private static void Set_Status(SPListItem item)
        {
            if (BLL.Tools.Get_Text(item, "enumStatusZadania").Equals("Nowe")
                && BLL.Tools.Get_Date(item, "Created").CompareTo(BLL.Tools.Get_Date(item, "Modified")) != 0)
            {
                BLL.Tools.Set_Text(item, "enumStatusZadania", "Obsługa", true);
            }
        }

        private bool HasValue(SPListItem item, string col)
        {
            double v = BLL.Tools.Get_Value(item, col);
            if (v >= 0) return true;
            else return false;
        }

    }

    public enum StatusZadania
    {
        Obsługa,
        Gotowe,
        Wysyłka,
        Zakończone,
        Anulowane
    }
}
