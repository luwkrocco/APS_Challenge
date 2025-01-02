using System.Alerts.Monitor.Provider.DAL.Models;

namespace System.Alerts.Monitor.Provider.DAL.Interfaces
{
    public interface ISystemAlertDAL
    {
        ValueTask<DALListSystemAlertsResponseModel> DALListSystemAlerts(DALListSystemAlertsRequestModel request);
        ValueTask<bool> DALRefreshSQLITE();
    }
}
