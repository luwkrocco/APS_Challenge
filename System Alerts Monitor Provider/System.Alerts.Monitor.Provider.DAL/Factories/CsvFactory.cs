using System.Alerts.Monitor.Common;
using System.Globalization;

namespace System.Alerts.Monitor.Provider.DAL.Factories
{
    // BASIC CSV READER METHOD TAKEN OFF CHATGPT
    public class CsvFactory(string path, BasicLogToFile logToFile)
    {
        private readonly string _path = path;
        private readonly BasicLogToFile _logToFile = logToFile;

        public string CSVPATH { get { return _path; } }

        public async Task<List<SystemAlert>> ReadSystemAlerts() => await ReadSystemAlerts(this._path);

        public async Task<List<SystemAlert>> ReadSystemAlerts(string filePath)
        {
            var alertLogs = new List<SystemAlert>();

            try
            {
                using var reader = new StreamReader(filePath);

                string? headerLine = await reader.ReadLineAsync(); // Read header
                if (string.IsNullOrEmpty(headerLine))
                    throw new InvalidOperationException("CSV file is empty or missing a header.");

                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length < 4) // Ensure correct number of fields
                        continue;

                    SystemAlertType alertType = SystemAlertType.NA;
                    if (!Enum.TryParse<SystemAlertType>(parts[2].Trim(), true, out alertType))
                        alertType = SystemAlertType.NA;


                    alertLogs.Add(new SystemAlert
                    {
                        Datetime = DateTime.ParseExact(parts[0], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                        SystemName = parts[1].Trim(),
                        AlertType = alertType,
                        FurtherDetails = parts[3].Trim()
                    });
                }
            }
            catch (Exception ex)
            {
                await this._logToFile.LogException(ex);
                alertLogs = new List<SystemAlert>();
            }

            return alertLogs;
        }
    }
}