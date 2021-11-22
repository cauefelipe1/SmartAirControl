using MediatR;
using SmartAirControl.API.Features.Device;
using SmartAirControl.Models.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAirControl.API.Features.DeviceAlert
{
    public partial class DeviceAlertMediator
    {
        public class DeviceAlertUpdateRequest : IRequest
        {
            public IEnumerable<DeviceAlertModel> Models { get; }

            public DeviceAlertUpdateRequest(IEnumerable<DeviceAlertModel> models)
            {
                Models = models;
            }
        }

        public class DeviceAlertUpdateHandler : IRequestHandler<DeviceAlertUpdateRequest>
        {
            private readonly IDeviceAlertRepository _repo;

            public DeviceAlertUpdateHandler(IDeviceAlertRepository repository)
            {
                _repo = repository;
            }

            public Task<Unit> Handle(DeviceAlertUpdateRequest request, CancellationToken cancellationToken) => Task.Run(() =>
            {
                var recodsToBeUpdated = new List<DeviceAlertDTO>(request.Models.Count());

                foreach (var model in request.Models)
                {
                    ValidateModel(model);

                    var dto = BuildDTO(model);

                    recodsToBeUpdated.Add(dto);
                }

                _repo.UpdateDeviceAlert(recodsToBeUpdated);

                return Unit.Value;
            });

            public void ValidateModel(DeviceAlertModel model)
            {
                if (model is null)
                    throw new Exception("Invalid model instance.");

                if (model.DeviceId <= 0)
                    throw new ArgumentNullException(nameof(model.DeviceId), "Device id must be informed.");
            }

            private DeviceAlertDTO BuildDTO(DeviceAlertModel model)
            {
                var dto = new DeviceAlertDTO
                {
                    DeviceAlertId = model.DeviceAlertId,
                    DeviceId = model.DeviceId,
                    Type = (int)model.AlertType,
                    InitialDeviceReportId = model.InitialDeviceReportId,
                    LatestDeviceReportId = model.LatestDeviceReportId,
                    AlertMessage = model.Message,
                    ResolveStatus = (int)model.ResolveStatus,
                    ViewStatus = (int)model.VisualizationStatus,
                    InsertTS = model.InsertTimestamp,
                    ResolveTS = model.ResolveStatus == DeviceAlertResolveStatus.New ? null : model.ResolveTimestamp
                };

                return dto;
            }
        }
    }
}
