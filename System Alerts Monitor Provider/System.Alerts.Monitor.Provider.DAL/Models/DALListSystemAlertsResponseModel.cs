
using System.Alerts.Monitor.Common;

namespace System.Alerts.Monitor.Provider.DAL.Models
{
    public class DALListSystemAlertsResponseModel
    {
        public string Status { get; init; } = null!;
        public string? Error { get; set; } = null!;
        public List<SystemAlert> SystemAlerts { get; init; } = null!;
    }
}
