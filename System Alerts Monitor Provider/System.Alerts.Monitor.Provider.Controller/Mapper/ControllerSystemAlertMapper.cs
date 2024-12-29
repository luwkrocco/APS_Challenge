using Riok.Mapperly.Abstractions;
using System.Alerts.Monitor.Provider.Controller.Models;
using System.Alerts.Monitor.Provider.DAL.Models;

namespace System.Alerts.Monitor.Provider.GrpcService.Mapper
{
    [Mapper]
    public partial class ControllerSystemAlertMapper
    {
        public partial ControllerListSystemAlertsResponseModel ToControllerListSystemAlertsResponse(DALListSystemAlertsResponseModel model);

        public partial DALListSystemAlertsRequestModel ToDALListSystemAlertsRequest(ControllerListSystemAlertsRequestModel model);
    }
}
