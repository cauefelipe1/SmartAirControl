using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAirControl.API.Core.Extensions;
using SmartAirControl.Models.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAirControl.API.Features.DeviceReport
{
    [Route("api/device/report")]
    [ApiController]
    [Authorize]
    public class DeviceReportController : Controller
    {
        private readonly IMediator _mediator;

        public DeviceReportController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Saves a new report into the system.
        /// If the device info is not valid or not readable the system will generate an device alert.
        /// </summary>
        /// <param name="reportData"><see cref="DeviceReportInput"/> instance with device report info.</param>
        [HttpPost("registerReport")]
        public async Task<ActionResult> RegisterReport([FromBody] IEnumerable<DeviceReportInput> reportData)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var claims = this.GetIdentityClaims();

            await _mediator.Send(new DeviceReportMediator.DeviceReportSaveRequest(reportData, claims.UserId, claims.DeviceRegistrationId));

            return Ok();
        }

        /// <summary>
        /// Returns a aggregated list of sensor's values for a specific device.
        /// </summary>
        /// <param name="deviceId">Device id to get the sensor values.</param>
        /// <param name="startDate">Start date for the interval.</param>
        /// <param name="endDate">End date for the interval.</param>
        /// <returns></returns>
        [HttpGet("getDeviceSensorByDateRange")]
        public async Task<ActionResult<IEnumerable<DeviceReportAggregated>>> GetDeviceSensorByDateRange(int deviceId, DateTime startDate, DateTime endDate)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var key = new DeviceReportSensorByDateRangeKey
            {
                DeviceId = deviceId,
                StartDate = startDate,
                EndDate = endDate
            };

            var reports = await _mediator.Send(new DeviceReportMediator.DeviceReportQueryRequest(key));

            var (interval, perDay) = GetAggregationParams(startDate.Date, endDate.AddDays(1).AddTicks(-1));

            var aggregatedReports = GroupReports(reports, interval, perDay);

            return Ok(aggregatedReports.Take(100));
        }

        private (int interval, bool perDay) GetAggregationParams(DateTime startDate, DateTime endDate)
        {
            double totalDays = (endDate - startDate).TotalDays;

            switch (totalDays)
            {
                case 1:
                    return new(1, false);
                case 7:
                    return new(6, false);
                case 14:
                    return new(12, false);
                case 21:
                    return new(16, false);
                case 30:
                    return new(1, true);
                case 90:
                    return new(3, true);
                case 180:
                    return new(6, true);
                default:
                {
                    double totalHours = Math.Ceiling((endDate - startDate).TotalHours);
                    int inter = (int)Math.Ceiling((totalHours / 28.0));

                    if (inter < 24)
                        return new(inter, false);
                    else
                        return new(inter, true);
                }
            }
        }

        private List<DeviceReportAggregated> GroupReports(IEnumerable<DeviceReportModel> reports, int interval, bool perDay)
        {
            var result = new List<DeviceReportAggregated>();

            var sensorGroup = reports.GroupBy(r => r.SensorType);

            foreach (var sensor in sensorGroup)
            {
                var sensorAggregated = sensor.GroupBy(x =>
                {
                    var stamp = x.DeviceReadTS.Value;
                    if (perDay)
                    {
                        stamp = stamp.AddDays(-(stamp.Day % interval));
                        stamp = stamp.AddHours(-(stamp.Hour));
                    }
                    else
                    {
                        stamp = stamp.AddHours(-(stamp.Hour % interval));
                    }

                    stamp = stamp.AddMinutes(-(stamp.Minute));
                    stamp = stamp.AddMilliseconds(-stamp.Millisecond - 1000 * stamp.Second);
                    return stamp;
                });

                foreach (var agg in sensorAggregated)
                {
                    var reportAggregated = new DeviceReportAggregated()
                    {
                        SensorType = sensor.Key.Value,
                        StartDate = perDay ? agg.Key.Date : agg.Key,
                        EndDate = perDay ? agg.Key.Date.AddDays(interval) : agg.Key.AddHours(interval),
                        FirstValue = agg.First(r => r.DeviceReadTS == agg.Min(r => r.DeviceReadTS)).SensorValue.Value,
                        LastValue = agg.Last(r => r.DeviceReadTS == agg.Max(r => r.DeviceReadTS)).SensorValue.Value,
                        MinimumValue = agg.Min(r => r.SensorValue.Value),
                        AverageValue = Math.Round(agg.Average(r => r.SensorValue.Value), 2),
                        MaximumValue = agg.Max(r => r.SensorValue.Value)
                    };

                    result.Add(reportAggregated);
                }
            }

            return result;
        }
    }
}
