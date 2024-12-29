using Riok.Mapperly.Abstractions;
using System.Alerts.Monitor.Common.Protos;
using System.Alerts.Monitor.Dashboard.DAL.Models;

namespace System.Alerts.Monitor.Dashboard.DAL.Mapper
{
    [Mapper]
    public partial class DALMapper
    {
        public partial ListSystemAlertsResponse ToListSystemAlertsResponse(ListSystemAlertsResponseModel model);


        [MapperIgnoreSource(nameof(ListSystemAlertsResponse.HasError))]
        public partial ListSystemAlertsResponseModel ToDALListSystemAlertsResponse(ListSystemAlertsResponse model);

        private partial ListSystemAlertsRequest baseToListSystemAlertsRequest(ListSystemAlertsRequestModel model);
        public ListSystemAlertsRequest ToListSystemAlertsRequest(ListSystemAlertsRequestModel model)
        {
            var request = new ListSystemAlertsRequestModel()
            {
                From = model.From,
                To = model.To,
                System = string.IsNullOrEmpty(model.System) ? string.Empty : model.System
            };

            return baseToListSystemAlertsRequest(request);
        }
    }
}
