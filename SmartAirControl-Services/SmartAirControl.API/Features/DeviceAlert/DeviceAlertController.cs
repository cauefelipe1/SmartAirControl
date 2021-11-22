using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAirControl.API.Core.Pagination;
using SmartAirControl.Models.Base;
using SmartAirControl.Models.Device;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAirControl.API.Features.DeviceAlert
{
    /// <summary>
    /// Defines endpoints to be used in requests related to device alerts.
    /// </summary>
    [Route("api/device/alert")]
    [ApiController]
    [Authorize]
    public class DeviceAlertController : Controller
    {
        private readonly IMediator _mediator;

        public DeviceAlertController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Returns a list of device alerts.
        /// </summary>
        /// <param name="unviewed">When true indicates that only not viewed alerts will be returned.</param>
        /// <param name="unresolved">When true indicates that only not solved alerts will be returned.</param>
        /// <param name="descending">When true the list is ordered in a descending order.</param>
        /// <param name="pageSize">Page size for the request.</param>
        /// <param name="page">Page for the request.</param>
        [HttpGet("getAlerts")]
        public async Task<ActionResult<PagedResponse<DeviceAlertModel>>> GetDeviceSensorByDateRange(bool unviewed, bool unresolved, bool descending, int pageSize = 100, int page = 1)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var key = new DeviceAlertResolveViewStatusKey
            {
                ResolveStatus = unresolved ?
                                new[] { DeviceAlertResolveStatus.New } :
                                new[] { DeviceAlertResolveStatus.New, DeviceAlertResolveStatus.Ignored, DeviceAlertResolveStatus.Resolved },
                ViewStatus = unviewed ? new[] { DeviceAlertViewStatus.New } : new[] { DeviceAlertViewStatus.New, DeviceAlertViewStatus.Viewd }
            };

            var alerts = await _mediator.Send(new DeviceAlertMediator.DeviceAlertQueryRequest(key));

            if (descending)
                alerts = alerts.OrderByDescending(a => a.InsertTimestamp);
            else
                alerts = alerts.OrderBy(a => a.InsertTimestamp);

            var paged = PaginationUtils.CreatedPagedResponse(alerts, pageSize, page);

            return Ok(paged);
        }

        /// <summary>
        /// Marks an alert as viewed.
        /// </summary>
        /// <param name="deviceAlertId">Id of alert to be marked as viewed.</param>
        [HttpPut("markAlertAsViewed/{deviceAlertId}")]
        public async Task<ActionResult> MarkAlertAsViewed(int deviceAlertId)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var alerts = await _mediator.Send(new DeviceAlertMediator.DeviceAlertQueryRequest(new DeviceAlertIdKey { DeviceAlertId = deviceAlertId }));
            var alert = alerts.FirstOrDefault();

            if (alert is not null)
            {
                alert.VisualizationStatus = DeviceAlertViewStatus.Viewd;
                await _mediator.Send(new DeviceAlertMediator.DeviceAlertUpdateRequest(new[] { alert }));
            }

            return Ok();
        }

        /// <summary>
        /// Marks an alert as ignored.
        /// </summary>
        /// <param name="deviceAlertId">Id of alert to be marked as ignored.</param>
        [HttpPut("markAlertAsIgnored/{deviceAlertId}")]
        public async Task<ActionResult> MarkAlertAsIgnored(int deviceAlertId)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var alerts = await _mediator.Send(new DeviceAlertMediator.DeviceAlertQueryRequest(new DeviceAlertIdKey { DeviceAlertId = deviceAlertId }));
            var alert = alerts.FirstOrDefault();

            if (alert is not null)
            {
                alert.ResolveStatus = DeviceAlertResolveStatus.Ignored;
                await _mediator.Send(new DeviceAlertMediator.DeviceAlertUpdateRequest(new[] { alert }));
            }

            return Ok();
        }
    }
}
