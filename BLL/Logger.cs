using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace BLL
{
    public class Logger
    {
        public static void LogEvent(string subject, string body)
        {
            SPDiagnosticsService diagSvc = SPDiagnosticsService.Local;

            diagSvc.WriteTrace(0,
                new SPDiagnosticsCategory("STAFix category", TraceSeverity.Monitorable, EventSeverity.Information),
                TraceSeverity.Monitorable,
                subject.ToString() + "{0}",
                new object[] { body.ToString() });
        }


        public static void LogEvent_EventReceiverInitiated(SPListItem item)
        {
            BLL.Logger.LogEvent(item.Web.Name,
                                string.Format("{0}.{1}.{2}.start",
                                item.ParentList.Title,
                                item.ContentType.Name,
                                item.ID.ToString()));
        }

        public static void LogEvent_EventReceiverCompleted(SPListItem item)
        {
            BLL.Logger.LogEvent(item.Web.Name,
                                string.Format("{0}.{1}.{2}.end",
                                item.ParentList.Title,
                                item.ContentType.Name,
                                item.ID.ToString()));
        }

        public static void LogError(string subject, string body, Exception ex = null)
        {
            SPDiagnosticsService diagSvc = SPDiagnosticsService.Local;

            diagSvc.WriteTrace(0,
                new SPDiagnosticsCategory("STAFix category", TraceSeverity.Unexpected, EventSeverity.Error),
                TraceSeverity.Monitorable,
                subject.ToString() + "{0}",
                new object[] { body.ToString() });

            if (ex!=null)
            {
                ElasticEmail.EmailGenerator.ReportError(ex, ex.TargetSite.ToString());
            }
        }

        public static void LogEvent_Procedure(string procName, SPListItem item, string remarks)
        {
            BLL.Logger.LogEvent(item.Web.Name,
                                string.Format("{0}.{1}.{2}",
                                procName,
                                item.ID.ToString(),
                                remarks));
        }
    }
}
