using System.Alerts.Monitor.Common;
using System.Alerts.Monitor.Provider.DAL.Factories;
using System.Alerts.Monitor.Provider.DAL.Interfaces;
using System.Alerts.Monitor.Provider.DAL.Models;
using System.Numerics;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace System.Alerts.Monitor.Provider.DAL.Implementations
{
    public class SystemAlertDAL(CsvFactory csvFactory, SQLiteFactory sqlFactory, IOptions<SAMOptions> options, BasicLogToFile logToFile) : ISystemAlertDAL
    {
        private BasicLogToFile _logToFile = logToFile;
        private CsvFactory _csvFactory = csvFactory;
        private SQLiteFactory _sqlFactory = sqlFactory;
        private ServiceOperationMode _serviceOperationMode = options.Value.ServiceOpeartionMode;

        public async ValueTask<DALListSystemAlertsResponseModel> DALListSystemAlerts(DALListSystemAlertsRequestModel request)
        {
            try
            {
                List<SystemAlert> results = null!;

                switch (_serviceOperationMode)
                {
                    case ServiceOperationMode.CSV: results = await _csvFactory.ReadSystemAlerts(); break;
                    case ServiceOperationMode.SQLITE: results = await _sqlFactory.ReadSystemAlerts(request); break;
                    default: throw new NotImplementedException($"Service Operation Mode '{_serviceOperationMode.ToString()}' Not Implemented!");
                }
                //READ FILE


                if (results == null)
                    return new DALListSystemAlertsResponseModel()
                    {
                        Status = "OK",
                        Error = "Could not Read System Alerts from CSV",
                        SystemAlerts = null!
                    };

                results = results.OrderByDescending(o => o.Datetime).ToList();

                return new DALListSystemAlertsResponseModel()
                {
                    Status = "OK",
                    Error = "N/A",
                    SystemAlerts = results
                };
            }
            catch (Exception ex)
            {
                await this._logToFile.LogException(ex);
                return new DALListSystemAlertsResponseModel() { Status = "ERROR", Error = ex.Message };
            }
        }

        public async ValueTask<bool> DALRefreshSQLITE()
        {
            try
            {
                var result = await _sqlFactory.RefreshSystemAlertsFromCsvAsync();
                return true;
            }
            catch (Exception ex)
            {
                await this._logToFile.LogException(ex);
                return false;
            }
        }
    }
}
