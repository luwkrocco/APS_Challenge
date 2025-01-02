
namespace System.Alerts.Monitor.Provider.DAL.Models
{
    public class DALListSystemAlertsRequestModel
    {
        public DateTime From { get; init; } = DateTime.MinValue;
        public DateTime To { get; init; } = DateTime.MaxValue;
        public string? System { get; init; } = null!;
    }
}
