using System.Alerts.Monitor.Common;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace System.Alerts.Monitor.Dashboard.UI.Models
{
    public class MonitoringData
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [JsonPropertyName("From")]
        public DateTime From { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [JsonPropertyName("To")]
        public DateTime To { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);

        public string SystemName { get; set; } = null!;

        [JsonPropertyName("AlertData")]
        public List<SystemAlert> AlertData { get; set; } = [];
    }
}
