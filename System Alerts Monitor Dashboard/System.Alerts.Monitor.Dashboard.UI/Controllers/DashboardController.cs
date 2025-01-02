using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Alerts.Monitor.Common;
using System.Alerts.Monitor.Dashboard.DAL;
using System.Alerts.Monitor.Dashboard.UI.Models;

namespace System.Alerts.Monitor.Dashboard.UI.Controllers
{
    public class DashboardController(DALController controller, BasicLogToFile logger) : Controller
    {
        private readonly BasicLogToFile _logger = logger;
        private readonly DALController _controller = controller;

        private MonitoringData _data { get; set; } = new();

        public bool RefreshData()
        {
            try
            {
                var results = _controller.ListSystemAlerts(_data.From, _data.To, _data.SystemName);
                _data.AlertData = results.SystemAlerts;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex).RunSynchronously();
                throw;
            }
        }
        public IActionResult Monitor()
        {
            return (RefreshData()) ? View(_data) : View();
        }

        [HttpPost]
        public IActionResult FilterData([FromBody] FilterModel filter)
        {
            try
            {
                _data.From = DateTime.TryParse(filter.FromDate, out DateTime from) ? from : _data.From;
                _data.To = DateTime.TryParse(filter.ToDate, out DateTime to) ? to : _data.To;
                _data.SystemName = string.IsNullOrEmpty(filter.SystemName) ? _data.SystemName : filter.SystemName;

                return (RefreshData()) ? Json(_data) : Json(null);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex).RunSynchronously();
                throw; 
            }
        }
    }
}