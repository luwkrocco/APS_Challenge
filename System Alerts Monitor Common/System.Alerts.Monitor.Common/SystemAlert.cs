using System.Text.Json.Serialization;

namespace System.Alerts.Monitor.Common
{
    public class SystemAlert
    {
        [JsonPropertyName("Datetime")]
        public DateTime Datetime { get; set; }

        [JsonIgnore]
        public SystemAlertType AlertType { get; set; }

        [JsonPropertyName("AlertType")]
        public string AlertTypeString { get { return AlertType.ToString(); } }

        [JsonPropertyName("SystemName")]
        public string SystemName { get; set; } = null!;

        [JsonPropertyName("FurtherDetails")]
        public string FurtherDetails { get; set; } = null!;
    }

}
