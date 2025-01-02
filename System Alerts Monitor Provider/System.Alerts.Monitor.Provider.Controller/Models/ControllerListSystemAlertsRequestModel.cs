
namespace System.Alerts.Monitor.Provider.Controller.Models
{
    public class ControllerListSystemAlertsRequestModel
    {
        public DateTime From { get; init; } = DateTime.MinValue;
        public DateTime To { get; init; } = DateTime.MaxValue;
        public string? System { get; init; } = null!;
    }
}
