using System.Alerts.Monitor.Provider.Controller.Models;

namespace System.Alerts.Monitor.Provider.Controller.Interfaces
{
    public interface ISystemAlertController
    {
        ValueTask<ControllerListSystemAlertsResponseModel> ControllerListSystemAlerts(ControllerListSystemAlertsRequestModel request);
        ValueTask<bool> ControllerRefreshSQLITE();
    }
}
