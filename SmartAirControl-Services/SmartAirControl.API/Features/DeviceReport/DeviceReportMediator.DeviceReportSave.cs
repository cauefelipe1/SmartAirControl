using MediatR;
using SmartAirControl.API.Features.Device;
using SmartAirControl.API.Features.DeviceAlert;
using SmartAirControl.Models.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SmartAirControl.Models.Common.Constants.Device;

namespace SmartAirControl.API.Features.DeviceReport
{
    public partial class DeviceReportMediator
    {
        private class ValidationResult
        {
            public bool IsParseable { get; set; } = true;

            public bool IsValid { get; set; } = true;

            public DeviceAlertType? AlertType { get; set; } = null;

            public string Message { get; set; } = null;
        }

        public class DeviceReportSaveRequest : IRequest
        {
            public IEnumerable<DeviceReportInput> Input { get; }

            public int DeviceId { get; }

            public int DeviceRegistrationId { get; }

            public DeviceReportSaveRequest(IEnumerable<DeviceReportInput> input, int deviceId, int deviceRegistrationId)
            {
                Input = input;
                DeviceId = deviceId;
                DeviceRegistrationId = deviceRegistrationId;
            }
        }

        public class DeviceReportSaveHandler : IRequestHandler<DeviceReportSaveRequest>
        {
            private readonly IDeviceReportRepository _repo;
            private readonly IMediator _mediator;

            public DeviceReportSaveHandler(IDeviceReportRepository repository, IMediator mediator)
            {
                _repo = repository;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(DeviceReportSaveRequest request, CancellationToken cancellationToken)
            {
                var deviceAlerts = new List<DeviceAlertSaveInput>();

                foreach (var input in request.Input)
                {
                    var validationResult = ValidateModel(input);

                    var dto = BuildDTO(input, validationResult, request.DeviceId, request.DeviceRegistrationId);

                    int reportId = _repo.SaveDeviceReport(dto);

                    if (validationResult.IsParseable)
                    {

                        AddAlertReports(validationResult,
                                    validationResult.IsValid ? 0 : reportId,
                                    request.DeviceId,
                                    input,
                                    deviceAlerts);
                    }
                    else
                    {
                        var key = new DeviceReportTypeRegistrationIdKey
                        {
                            DeviceRegistrationId = request.DeviceRegistrationId,
                            ReportType = ReportyType.Unreadable
                        };

                        int unreadableRptCount = _repo.GetDeviceReportTypeRegistrationIdCount(key);

                        if (unreadableRptCount > UNREADABLE_REPORTS_LIMIT)
                        {
                            AddAlertReports(validationResult, 0, request.DeviceId, input, deviceAlerts);
                        }
                    }
                }

                if (deviceAlerts.Count > 0)
                    _ = await _mediator.Send(new DeviceAlertMediator.DeviceAlertSaveRequest(deviceAlerts));

                return Unit.Value;
            }

            private void AddAlertReports(
                ValidationResult validationResult,
                int reportId,
                int deviceId,
                DeviceReportInput input,
                List<DeviceAlertSaveInput> alertReports)
            {

                if (validationResult.AlertType.HasValue)
                {
                    alertReports.Add(new()
                    {
                        DeviceId = deviceId,
                        StatusReportId = reportId,
                        Type = validationResult.AlertType.Value,
                        Message = validationResult.Message
                    });

                    return;
                }

                if (!validationResult.IsParseable)
                {
                    alertReports.Add(new()
                    {
                        DeviceId = deviceId,
                        StatusReportId = reportId,
                        Type = DeviceAlertType.UnreadableData,
                        Message = "Device sending unintelligible data."
                    });

                    return;
                }

                var report = new DeviceAlertSaveInput
                {
                    DeviceId = deviceId,
                    StatusReportId = 0,
                };

                if (input.ReportType == ReportyType.SensorReading)
                {
                    report.Type = GetRangeAlertType(input.SensorType.Value);
                    alertReports.Add(report);

                    if (input.SensorType == SensorType.CarbonMonoxide)
                    {
                        alertReports.Add(new()
                        {
                            DeviceId = deviceId,
                            StatusReportId = 0,
                            Type = DeviceAlertType.CarbonDanger
                        });
                    }
                }
                else
                {
                    report.Type = DeviceAlertType.HealthProblem;
                    alertReports.Add(report);
                }
            }

            private ValidationResult ValidateModel(DeviceReportInput input)
            {
                var result = new ValidationResult();

                if (input is null)
                {
                    result.Message = "Device sent an invalid mode (null).";
                    result.IsParseable = false;
                    result.IsValid = false;
                }

                if (input.SnapshotTS is null || input.SnapshotTS == DateTimeOffset.MinValue)
                {
                    result.Message = "Device did not sent a valid snapshot.";
                    result.IsParseable = false;
                    result.IsValid = false;
                }

                if (input.ReportType != ReportyType.SensorReading && input.ReportType != ReportyType.HealthCheck)
                {
                    result.Message = $"Device sent an invalid report type: {(int)input.ReportType}.";
                    result.IsParseable = false;
                    result.IsValid = false;
                }

                if (input.ReportType == ReportyType.SensorReading && !input.SensorType.HasValue)
                {
                    result.Message = "Device sent a sensor reading report but did not sent the sensor type.";
                    result.IsParseable = false;
                    result.IsValid = false;
                }

                if (result.IsParseable && result.IsValid)
                {
                    if (input.ReportType == ReportyType.SensorReading)
                        ValidateSensorReading(input, result);

                    else
                        ValidateHealthCheck(input, result);
                }

                return result;
            }

            private void ValidateSensorReading(DeviceReportInput input, ValidationResult validationResult)
            {
                if (double.TryParse(input.SensorValue, out double val))
                {
                    var sensorRange = Sensors.SENSOR_RANGES[input.SensorType.Value];

                    if (val < sensorRange.MinValue || val > sensorRange.MaxValue)
                    {
                        validationResult.AlertType = GetRangeAlertType(input.SensorType.Value);
                        validationResult.Message = $"Sensor {input.SensorType.Value} has value out of range.";
                        validationResult.IsValid = false;

                    }

                    if (input.SensorType == SensorType.CarbonMonoxide && val > Sensors.CARBON_THRESHOLD)
                    {
                        validationResult.AlertType = DeviceAlertType.CarbonDanger;
                        validationResult.Message = "CO value has exceeded danger limit.";
                        validationResult.IsValid = false;
                    }
                }
                else
                {
                    validationResult.Message = $"{input.SensorValue:500}";//   input.SensorValue.Length > 500 ? input.SensorValue.Substring(0, 500) : input.SensorValue;
                    validationResult.IsParseable = false;
                    validationResult.IsValid = false;
                }
            }

            private void ValidateHealthCheck(DeviceReportInput input, ValidationResult validationResult)
            {
                if (Health.HEALTH_STATUS_VALUES.Any(hv => string.Equals(hv, input.SensorValue, StringComparison.OrdinalIgnoreCase)))
                {
                    if (!Health.OK.Equals(input.SensorValue, StringComparison.OrdinalIgnoreCase))
                    {
                        validationResult.AlertType = DeviceAlertType.HealthProblem;
                        validationResult.Message = "Device is reporting health problem.";
                        validationResult.IsValid = false;
                    }
                }
                else
                {
                    validationResult.Message = $"Not know health status: {input.SensorValue:500}";
                    validationResult.IsParseable = false;
                    validationResult.IsValid = false;
                }
            }

            private DeviceAlertType GetRangeAlertType(SensorType sensorType)
            {
                if (sensorType == SensorType.Temperature)
                    return DeviceAlertType.TemperatureRange;

                else if (sensorType == SensorType.Humidity)
                    return DeviceAlertType.HumidtyRange;

                else
                    return DeviceAlertType.CarbonRange;
            }

            private DeviceReportDTO BuildDTO(DeviceReportInput input, ValidationResult validation, int deviceId, int registrationId)
            {
                var dto = new DeviceReportDTO
                {
                    DeviceId = deviceId,
                    DeviceRegistrationId = registrationId,
                    InsertTS = DateTime.UtcNow
                };

                if (validation.IsParseable)
                {
                    dto.ReportType = (int)input.ReportType;
                    dto.DeviceReadTS = input.SnapshotTS.Value;

                    if (input.ReportType == ReportyType.SensorReading)
                    {
                        dto.SensorValue = double.Parse(input.SensorValue);
                        dto.SensorType = (int)input.SensorType.Value;
                        dto.HealthStatus = null;
                    }
                    else
                    {
                        dto.SensorValue = null;
                        dto.SensorType = null;
                        dto.HealthStatus = input.SensorValue;
                    }
                }
                else
                {
                    dto.ReportType = (int)ReportyType.Unreadable;
                    dto.SensorType = input.SensorType.HasValue ? (int)input.SensorType : null;
                    dto.DeviceReadTS = null;
                    dto.Message = validation.Message;
                }

                return dto;
            }
        }
    }
}
