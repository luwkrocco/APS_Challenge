using Riok.Mapperly.Abstractions;
using System.Alerts.Monitor.Common.Protos;
using System.Alerts.Monitor.Provider.Controller.Models;

namespace System.Alerts.Monitor.Provider.GrpcService.Mapper
{
    [Mapper]
    public partial class SystemAlertMapper
    {
        public partial ListSystemAlertsResponse ToListSystemAlertsResponse(ControllerListSystemAlertsResponseModel model);

        [MapperIgnoreSource(nameof(ListSystemAlertsRequest.HasSystem))]
        public partial ControllerListSystemAlertsRequestModel ToControllerListSystemAlertsRequest(ListSystemAlertsRequest model);
    }
}
