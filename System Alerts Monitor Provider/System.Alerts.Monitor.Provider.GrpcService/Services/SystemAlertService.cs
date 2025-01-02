
using Grpc.Core;
using System.Alerts.Monitor.Common;
using System.Alerts.Monitor.Common.Protos;
using System.Alerts.Monitor.Provider.Controller.Interfaces;
using System.Alerts.Monitor.Provider.GrpcService.Mapper;

namespace System.Alerts.Monitor.Provider.GrpcService.Services
{
    public class SystemAlertService(ISystemAlertController systemAlertController, SystemAlertMapper systemAlertMapper, BasicLogToFile logToFile) : System.Alerts.Monitor.Common.Protos.SystemAlertService.SystemAlertServiceBase
    {
        private readonly ISystemAlertController _systemAlertController = systemAlertController;
        private readonly SystemAlertMapper _systemAlertMapper = systemAlertMapper;
        private readonly BasicLogToFile _logToFile = logToFile;

        public override async Task<ListSystemAlertsResponse> ListSystemAlerts(ListSystemAlertsRequest request, ServerCallContext context)
        {
            try
            {
                var crequest = this._systemAlertMapper.ToControllerListSystemAlertsRequest(request);
                var cresponse = await this._systemAlertController.ControllerListSystemAlerts(crequest);
                return this._systemAlertMapper.ToListSystemAlertsResponse(cresponse);
            }
            catch (Exception ex)
            {
                await this._logToFile.LogException(ex);
                return new ListSystemAlertsResponse() { Status = "ERROR", Error = ex.Message };
            }
        }

        public override async Task<BoolResponse> RefreshSQLITE(Empty request, ServerCallContext context)
        {
            try
            {
                var response = await this._systemAlertController.ControllerRefreshSQLITE();
                return new BoolResponse() { Result = response };
            }
            catch (Exception ex)
            {
                await this._logToFile.LogException(ex);
                return new BoolResponse() { Result = false };
            }
        }
    }
}
