using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace EventReceivers.admProcesy
{
    class ObslugaKartKontrolnych
    {
        const string targetList = @"Karty kontrolne";
        private static object _STATUS_ZADANIA_ZWOLNIONE = "Zwolnione do wysyłki";
  

        /// <summary>
        /// Zastąpiony przez Workflow
        /// </summary>
        /// <param name="item"></param>
        internal static void Execute(Microsoft.SharePoint.SPListItem item)
        {
            BLL.Logger.LogEvent("Obsługa kart kontrolnych", item.ID.ToString());

            SPList list = item.Web.Lists.TryGetList(targetList);

            //ustaw okres karencji
            int offset = int.Parse(BLL.admSetup.GetValue(item.Web, "KK_IGNORE_UPDATES_MINUTES"));
            DateTime targetDate = DateTime.Now.AddMinutes(-1 * offset);

            list.Items.Cast<SPListItem>()
                .Where(i => BLL.Tools.Get_Date(i, "Modified") < targetDate) //nie ruszaj modyfikowanych w ciągu istatnicj x minut
                .Where(i => BLL.Tools.Get_Text(i, "colZUS_StatusZadania").Equals(_STATUS_ZADANIA_ZWOLNIONE)
                            || BLL.Tools.Get_Text(i, "colPD_StatusZadania").Equals(_STATUS_ZADANIA_ZWOLNIONE)
                            || BLL.Tools.Get_Text(i, "colVAT_StatusZadania").Equals(_STATUS_ZADANIA_ZWOLNIONE)
                            || BLL.Tools.Get_Text(i, "colRBR_StatusZadania").Equals(_STATUS_ZADANIA_ZWOLNIONE))

                .ToList()
                .ForEach(i =>
                {
                    BLL.Workflows.StartWorkflow(i, "Przygotuj wiadomość dla klienta");
                    BLL.Logger.LogEvent(BLL.Tools.Get_LookupValue(i, "selKlient").ToString(), i.ID.ToString());
                });
        }
    }
}
