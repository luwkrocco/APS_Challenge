using Microsoft.Data.Sqlite;
using System.Alerts.Monitor.Common;
using System.Alerts.Monitor.Provider.DAL.Models;

namespace System.Alerts.Monitor.Provider.DAL.Factories
{
    // BASIC SQL LITE IMPLEMENTATION METHOD ADAPTED OFF CHATGPT RESULTS
    public class SQLiteFactory(string path, CsvFactory csvFactory, BasicLogToFile logToFile)
    {
        private readonly string _path = path;
        private readonly BasicLogToFile _logToFile = logToFile;
        private readonly CsvFactory _csvFactory = csvFactory;

        private const string SYSTEM_ALERTS_TABLE = "tb_SystemAlerts";

        #region ReadSystemAlerts
        public async Task<List<SystemAlert>> ReadSystemAlerts() => await ReadSystemAlerts(new DALListSystemAlertsRequestModel() { });
        public async Task<List<SystemAlert>> ReadSystemAlerts(DALListSystemAlertsRequestModel request) => await ReadSystemAlerts(this._path, request);
        public async Task<List<SystemAlert>> ReadSystemAlerts(string sqlitePath, DALListSystemAlertsRequestModel request)
        {
            List<SystemAlert> alertLogs = [];

            try
            {
                if (!File.Exists(sqlitePath))
                    throw new FileNotFoundException($"SQLITE DATABASE NOT FOUND AT '{sqlitePath}'!");

                string from = request.From.ToString("yyyy-MM-dd HH:mm:ss");
                string to = request.To.ToString("yyyy-MM-dd HH:mm:ss");

                var selectQuery = @$" 
                        SELECT Datetime, AlertType, SystemName, FurtherDetails FROM {SYSTEM_ALERTS_TABLE} 
                        WHERE 
                            Datetime >= '{from}' AND Datetime <= '{to}'
                                AND
                            ( SystemName = '{request.System}' OR '{request.System}' = '' OR '{request.System}' = 'ALL' ) ";

                // OPEN CONNECTION TO DB
                using (var connection = new SqliteConnection($"Data Source={sqlitePath};"))
                using (var selectCommand = new SqliteCommand(selectQuery, connection))
                {
                    connection.Open();
                    using (var resultsReader = selectCommand.ExecuteReader())
                        while (resultsReader.Read())
                            alertLogs.Add(new SystemAlert()
                            {
                                Datetime = resultsReader.GetDateTime(resultsReader.GetOrdinal("Datetime")),
                                AlertType = Enum.TryParse<SystemAlertType>(resultsReader["AlertType"].ToString(), out var alertType) ? alertType : SystemAlertType.NA,
                                SystemName = resultsReader["SystemName"].ToString() ?? string.Empty,
                                FurtherDetails = resultsReader["FurtherDetails"].ToString() ?? string.Empty,
                            });
                }
            }
            catch (Exception ex)
            {
                await this._logToFile.LogException(ex);
                alertLogs = [];
            }

            return alertLogs;
        }
        #endregion

        #region RefreshSystemAlertsFromCsvAsync
        public async Task<List<SystemAlert>> RefreshSystemAlertsFromCsvAsync() => await RefreshSystemAlertsFromCsvAsync(this._csvFactory.CSVPATH, this._path);
        public async Task<List<SystemAlert>> RefreshSystemAlertsFromCsvAsync(string csvPath) => await RefreshSystemAlertsFromCsvAsync(csvPath, this._path);
        public async Task<List<SystemAlert>> RefreshSystemAlertsFromCsvAsync(string csvPath, string sqlitePath)
        {
            var alertLogs = new List<SystemAlert>();

            try
            {
                // Retrieve Latest System Alerts from CSV File.
                List<SystemAlert> alerts = await _csvFactory.ReadSystemAlerts(csvPath);

                // Ensure database file exists
                if (File.Exists(sqlitePath)) File.Delete(sqlitePath);
                if (!File.Exists(sqlitePath)) File.Create(sqlitePath).Dispose();

                // OPEN CONNECTION TO DB
                using (var connection = new SqliteConnection($"Data Source={sqlitePath};"))
                {
                    connection.Open();

                    // START TRANSACTION LOGIC
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            //DROP AND RECREATE TABLE IF NEEDED
                            await DropAndRecreateTable(connection, transaction);

                            //INSERT CSV DATA INTO TABLE
                            await InsertIntoTable(connection, transaction, alerts);

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                    }
                }

                return await ReadSystemAlerts();
            }
            catch (Exception ex)
            {
                await this._logToFile.LogException(ex);
                alertLogs = new List<SystemAlert>();
            }

            return alertLogs;
        }
        #endregion

        #region UTILITY METHODS
        private async Task<bool> DropAndRecreateTable(SqliteConnection connection, SqliteTransaction transaction)
        {
            try
            {
                // Drop the table if it exists
                var dropTableQuery = $"DROP TABLE IF EXISTS {SYSTEM_ALERTS_TABLE};";
                using (var command = new SqliteCommand(dropTableQuery, connection, transaction)) command.ExecuteNonQuery();

                // Create a new table
                var createTableQuery = @$"
                        CREATE TABLE {SYSTEM_ALERTS_TABLE} (
                            Datetime TEXT NOT NULL,
                            AlertType INTEGER NOT NULL,
                            SystemName TEXT NOT NULL,
                            FurtherDetails TEXT NOT NULL
                        );";

                using (var command = new SqliteCommand(createTableQuery, connection, transaction)) command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                await this._logToFile.LogMessage($"Error occured while attempting to Drop and Recreate table {SYSTEM_ALERTS_TABLE}!");
                throw;
            }
        }

        private async Task<bool> InsertIntoTable(SqliteConnection connection, SqliteTransaction transaction, List<SystemAlert> alerts)
        {
            try
            {
                var insertQuery = @$" INSERT INTO {SYSTEM_ALERTS_TABLE} (Datetime, AlertType, SystemName, FurtherDetails) VALUES (@Datetime, @AlertType, @SystemName, @FurtherDetails); ";
                using (var insertCommand = new SqliteCommand(insertQuery, connection, transaction))
                {
                    foreach (var alert in alerts)
                    {
                        insertCommand.Parameters.Clear();
                        insertCommand.Parameters.AddWithValue("@Datetime", alert.Datetime.ToString("yyyy-MM-dd HH:mm:ss"));
                        insertCommand.Parameters.AddWithValue("@AlertType", (int)alert.AlertType);
                        insertCommand.Parameters.AddWithValue("@SystemName", alert.SystemName);
                        insertCommand.Parameters.AddWithValue("@FurtherDetails", alert.FurtherDetails);

                        insertCommand.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch
            {
                await this._logToFile.LogMessage($"Error occured while attempting to Insert Alerts into table {SYSTEM_ALERTS_TABLE}!");
                throw;
            }
        }
        #endregion
    }
}
