using MediatR;
using SmartAirControl.API.Features.Device;
using SmartAirControl.Models.Base;
using SmartAirControl.Models.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAirControl.API.Features.DeviceReport
{
    public partial class DeviceReportMediator
    {
        public class DeviceReportQueryRequest : IRequest<IEnumerable<DeviceReportModel>>
        {
            public IModelKey<DeviceReportModel> Key { get; }

            public DeviceReportQueryRequest(IModelKey<DeviceReportModel> key)
            {
                Key = key;
            }
        }

        public class DeviceReportQueryHandler : IRequestHandler<DeviceReportQueryRequest, IEnumerable<DeviceReportModel>>
        {
            private readonly IDeviceReportRepository _repo;

            public DeviceReportQueryHandler(IDeviceReportRepository repository)
            {
                _repo = repository;
            }

            public Task<IEnumerable<DeviceReportModel>> Handle(DeviceReportQueryRequest request, CancellationToken cancellationToken) => Task.Run(() =>
            {
                IEnumerable<DeviceReportModel> result = null;

                if (request.Key is DeviceReportSensorByDateRangeKey sensorDateRangeKey)
                {
                    var dtos = _repo.GetDeviceReportSensorByDateRange(sensorDateRangeKey);

                    result = dtos.Select(dto => BuildModel(dto));
                }
                else
                {
                    throw new Exception($"Missing an implementation for {request.Key.GetType()}");
                }

                return result;
            });

            private DeviceReportModel BuildModel(DeviceReportDTO dto)
            {
                var model = new DeviceReportModel
                {
                    DeviceReportId = dto.DeviceReportId,
                    DeviceId = dto.DeviceId,
                    DeviceRegistrationId = dto.DeviceRegistrationId,
                    ReportType = (ReportyType)dto.ReportType,
                    SensorType = dto.SensorType.HasValue ? (SensorType)dto.SensorType.Value : null,
                    SensorValue = dto.SensorValue,
                    HealthStatus = dto.HealthStatus,
                    Message = dto.Message,
                    DeviceReadTS = dto.DeviceReadTS,
                    InsertTS = dto.InsertTS
                };

                return model;

            }
        }
    }
}
