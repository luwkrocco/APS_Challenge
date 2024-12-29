using Grpc.Net.ClientFactory;
using System.Alerts.Monitor.Common;
using System.Alerts.Monitor.Common.Protos;
using System.Alerts.Monitor.Dashboard.DAL.Interfaces;
using System.Alerts.Monitor.Dashboard.DAL.Mapper;
using System.Alerts.Monitor.Dashboard.DAL.Models;

namespace System.Alerts.Monitor.Dashboard.DAL.Services
{
    public class SAMProviderService(GrpcClientFactory grpcClientFactory, DALMapper systemAlertMapper, BasicLogToFile logToFile) : ISAMProviderService
    {
        private readonly BasicLogToFile _logToFile = logToFile;
        private readonly DALMapper _systemAlertMapper = systemAlertMapper;
        private readonly GrpcClientFactory _grpcClientFactory = grpcClientFactory;

        public async ValueTask<ListSystemAlertsResponseModel> ListSystemAlerts(ListSystemAlertsRequestModel dal_request)
        {
            try
            {
                var grpcClient = GetClient();

                var request = this._systemAlertMapper.ToListSystemAlertsRequest(dal_request);
                var result = await grpcClient.ListSystemAlertsAsync(request);

                return (result != null)
                    ? _systemAlertMapper.ToDALListSystemAlertsResponse(result)
                    : new ListSystemAlertsResponseModel() { Status = "NO DATA", Error = "NULL Result returned.", SystemAlerts = null! };
            }
            catch (Exception ex)
            {
                await this._logToFile.LogException(ex);
                return new ListSystemAlertsResponseModel() { Status = "ERROR", Error = ex.Message, SystemAlerts = null! };
            }
        }

        private SystemAlertService.SystemAlertServiceClient GetClient()
        {
            try
            {
                return _grpcClientFactory.CreateClient<SystemAlertService.SystemAlertServiceClient>("SystemAlertServiceClient");
            }
            catch
            {
                throw new Exception("Error occured while attempting to initialize the GRPC Service Client.");
            }
        }

    }
}