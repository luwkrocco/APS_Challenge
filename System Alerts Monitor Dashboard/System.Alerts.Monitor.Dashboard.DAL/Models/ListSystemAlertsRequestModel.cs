
namespace System.Alerts.Monitor.Dashboard.DAL.Models
{
    public class ListSystemAlertsRequestModel
    {
        public DateTime From { get; init; } = DateTime.MinValue;
        public DateTime To { get; init; } = DateTime.MaxValue;
        public string System { get; init; } = string.Empty;
    }
}
