using System.Alerts.Monitor.Common;
using System.Alerts.Monitor.Dashboard.DAL.Interfaces;
using System.Alerts.Monitor.Dashboard.DAL.Models;

namespace System.Alerts.Monitor.Dashboard.DAL
{
    public class DALController(ISAMProviderService providerService, BasicLogToFile logToFile)
    {
        private readonly BasicLogToFile _logToFile = logToFile;
        private readonly ISAMProviderService _providerService = providerService;

        public ListSystemAlertsResponseModel ListSystemAlerts(DateTime from, DateTime to, string system)
            => ListSystemAlerts(new ListSystemAlertsRequestModel() { From = from, To = to, System = system });

        public ListSystemAlertsResponseModel ListSystemAlerts(ListSystemAlertsRequestModel request)
        {
            try
            {
                var result = this._providerService.ListSystemAlerts(request).Result;
                return result;
            }//try
            catch (Exception ex)
            {
                this._logToFile.LogException(ex);
                return new ListSystemAlertsResponseModel() { Status = "ERROR", Error = ex.Message, SystemAlerts = null! };
            }
        }
    }
}
