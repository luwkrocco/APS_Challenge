
using System.Alerts.Monitor.Common;

namespace System.Alerts.Monitor.Dashboard.DAL.Models
{
    public class ListSystemAlertsResponseModel
    {
        public string Status { get; init; } = null!;
        public string? Error { get; set; } = null!;
        public List<SystemAlert> SystemAlerts { get; init; } = null!;
    }
}
