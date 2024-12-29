

using System.Alerts.Monitor.Dashboard.DAL.Models;

namespace System.Alerts.Monitor.Dashboard.DAL.Interfaces
{
    public interface ISAMProviderService
    {
        ValueTask<ListSystemAlertsResponseModel> ListSystemAlerts(ListSystemAlertsRequestModel request);
    }
}
