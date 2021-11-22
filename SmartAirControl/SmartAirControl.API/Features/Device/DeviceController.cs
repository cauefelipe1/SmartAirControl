using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAirControl.API.Core.Jwt;
using SmartAirControl.API.Core.Pagination;
using SmartAirControl.Models.Base;
using SmartAirControl.Models.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAirControl.API.Features.Device
{
    /// <summary>
    /// Defines endpoints to be used in requests related to devices.
    /// </summary>
    [Route("api/device")]
    [ApiController]
    [Authorize]
    public class DeviceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeviceController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Register a device in the system.
        /// </summary>
        /// <param name="deviceInfo"><see cref="RegisterDeviceInput"/> instance with the device info.</param>
        /// <returns>JWT token info to be used in the device requests.</returns>
        [AllowAnonymous]
        [HttpPost("registerDevice")]
        public async Task<ActionResult<TokenInfo>> RegisterDevice([FromBody] RegisterDeviceInput deviceInfo)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var token = await _mediator.Send(new DeviceReportMediator.DeviceRegistrationRequest(deviceInfo));

            return Ok(token);
        }

        /// <summary>
        /// Returns a lists of all devices.
        /// </summary>
        /// <param name="pageSize">Page size for the request.</param>
        /// <param name="page">Page for the request.</param>
        /// <returns>Pagineted list of devices.</returns>
        [HttpGet("getAll")]
        public async Task<ActionResult<PagedResponse<DeviceModel>>> GetAll(int pageSize = 100, int page = 1)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var devices = await _mediator.Send(new DeviceMediator.DeviceQueryRequest(new DeviceAllKey()));


            var paged = PaginationUtils.CreatedPagedResponse(devices, pageSize, page);

            return Ok(paged);
        }

        /// <summary>
        /// Returns a lists of all devices in a flat view.
        /// </summary>
        /// <param name="pageSize">Page size for the request.</param>
        /// <param name="page">Page for the request.</param>
        /// <returns>Pagineted list of devices flat view.</returns>
        [HttpGet("getAllFlatView")]
        public async Task<ActionResult<PagedResponse<DeviceWithRegistrationsFlatView>>> GetAllFlatView(int pageSize = 100, int page = 1)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var devices = await _mediator.Send(new DeviceMediator.DeviceQueryRequest(new DeviceAllKey()));

            var flatview = new List<DeviceWithRegistrationsFlatView>();
            foreach (var device in devices)
            {
                foreach (var registration in device.Registrations)
                {
                    flatview.Add(new()
                    {
                        DeviceId = device.DeviceId,
                        SerialNumber = device.SerialNumber,
                        ModelName = device.ModelName,
                        FirmwareVersion = registration.FirmwareVersion,
                        RegistrationTS = registration.RegistrationTS
                    });
                }
            }

            flatview = flatview.OrderByDescending(f => f.RegistrationTS)
                               .ToList();

            var paged = PaginationUtils.CreatedPagedResponse(flatview, pageSize, page);

            return Ok(paged);
        }

        /// <summary>
        /// Gets a device using te serial number.
        /// </summary>
        /// <param name="serialNumber">Device's serial number.</param>
        /// <returns>Device</returns>
        [HttpGet("getBySerialNumber")]
        public async Task<ActionResult<DeviceModel>> GetBySerialNumber(string serialNumber)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var devices = await _mediator.Send(new DeviceMediator.DeviceQueryRequest(new DeviceSerialNumberKey { SerialNumber = serialNumber }));

            var device = devices.FirstOrDefault();

            return Ok(device);
        }

        /// <summary>
        /// Returns a lists of all devices based on a start and end date range.
        /// </summary>
        /// <param name="startDate">Start date for the interval.</param>
        /// <param name="endDate">End date for the interval.</param>
        /// <param name="pageSize">Page size for the request.</param>
        /// <param name="page">Page for the request.</param>
        /// <returns>Pagineted list of devices.</returns>
        [HttpGet("getAllByDateRange")]
        public async Task<ActionResult<PagedResponse<DeviceModel>>> GetAll(DateTime startDate, DateTime endDate, int pageSize = 100, int page = 1)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var devices = await _mediator.Send(new DeviceMediator.DeviceQueryRequest(new DeviceAllKey()));

            var lEndDate = endDate.Date.AddDays(1).AddTicks(-1);

            devices = devices.Where(d => d.Registrations.Any(r => r.RegistrationTS >= startDate.Date && r.RegistrationTS <= lEndDate));

            var paged = PaginationUtils.CreatedPagedResponse(devices, pageSize, page);

            return Ok(paged);
        }
    }
}
