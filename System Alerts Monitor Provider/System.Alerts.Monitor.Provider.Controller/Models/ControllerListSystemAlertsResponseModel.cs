
using System.Alerts.Monitor.Common;

namespace System.Alerts.Monitor.Provider.Controller.Models
{
    public class ControllerListSystemAlertsResponseModel
    {
        public string Status { get; init; } = null!;
        public string? Error { get; set; } = null!;
        public List<SystemAlert> SystemAlerts { get; init; } = null!;
    }
}
