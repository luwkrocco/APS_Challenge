using System.Alerts.Monitor.Common;
using System.Alerts.Monitor.Provider.Controller.Interfaces;
using System.Alerts.Monitor.Provider.Controller.Models;
using System.Alerts.Monitor.Provider.DAL.Interfaces;
using System.Alerts.Monitor.Provider.GrpcService.Mapper;

namespace System.Alerts.Monitor.Provider.Controller.Implementation
{
    public class SystemAlertController(ISystemAlertDAL systemAlertDAL, ControllerSystemAlertMapper mapper, BasicLogToFile logToFile) : ISystemAlertController
    {
        private ISystemAlertDAL _systemAlertDAL = systemAlertDAL;
        private ControllerSystemAlertMapper _mapper = mapper;
        private BasicLogToFile _logToFile = logToFile;

        public async ValueTask<ControllerListSystemAlertsResponseModel> ControllerListSystemAlerts(ControllerListSystemAlertsRequestModel request)
        {
            try
            {
                var drequest = this._mapper.ToDALListSystemAlertsRequest(request);
                var dresponse = await this._systemAlertDAL.DALListSystemAlerts(drequest);
                return this._mapper.ToControllerListSystemAlertsResponse(dresponse);
            }
            catch (Exception ex)
            {
                await this._logToFile.LogException(ex);
                return new ControllerListSystemAlertsResponseModel() { Status = "ERROR", Error = ex.Message };
            }
        }

        public async ValueTask<bool> ControllerRefreshSQLITE()
        {
            try
            {
                var response = await this._systemAlertDAL.DALRefreshSQLITE();
                return response;
            }
            catch (Exception ex)
            {
                await this._logToFile.LogException(ex);
                return false;
            }
        }
    }
}
