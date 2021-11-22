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
        public class DeviceAlertSaveRequest : IRequest
        {
            public IEnumerable<DeviceAlertSaveInput> Models { get; }

            public DeviceAlertSaveRequest(IEnumerable<DeviceAlertSaveInput> models)
            {
                Models = models;
            }
        }

        public class DeviceAlertSaveHandler : IRequestHandler<DeviceAlertSaveRequest>
        {
            private readonly IDeviceAlertRepository _repo;

            public DeviceAlertSaveHandler(IDeviceAlertRepository repository)
            {
                _repo = repository;
            }

            public Task<Unit> Handle(DeviceAlertSaveRequest request, CancellationToken cancellationToken) => Task.Run(() =>
            {
                foreach (var model in request.Models)
                {
                    ValidateModel(model);

                    var key = new DeviceAlertTypeResolveStatusKey
                    {
                        DeviceId = model.DeviceId,
                        AlertType = model.Type,
                        ResolveStatus = DeviceAlertResolveStatus.New
                    };

                    var alertDtos = _repo.GetDeviceAlertTypeResolveStatus(key);
                    var alertDto = alertDtos.FirstOrDefault();

                    bool save = true;

                    if (alertDto is not null)
                    {
                        if (model.Type != DeviceAlertType.UnreadableData)
                            FillDTO(model, alertDto);
                        else
                            save = false;
                    }
                    else if (alertDto is null && (model.StatusReportId > 0 || model.Type == DeviceAlertType.UnreadableData))
                    {
                        alertDto = new DeviceAlertDTO();
                        FillDTO(model, alertDto);
                    }
                    else
                        save = false;

                    if (save)
                        _repo.SaveDeviceAlert(alertDto);
                }

                return Unit.Value;
            });

            public void ValidateModel(DeviceAlertSaveInput model)
            {
                if (model is null)
                    throw new Exception("Invalid model instance.");

                if (model.DeviceId <= 0)
                    throw new ArgumentNullException(nameof(model.DeviceId), "Device id must be informed.");

                if (model.StatusReportId > 0 && string.IsNullOrEmpty(model.Message))
                    throw new ArgumentNullException(nameof(model.Message), "When the status report is grater than 0 an alert message must be informed.");

                if ((int)model.Type <= 0 || (int)model.Type > 6)
                    throw new ArgumentNullException(nameof(model.Type), "Alert type informed is not valid.");
            }

            private void FillDTO(DeviceAlertSaveInput model, DeviceAlertDTO dto)
            {
                if (model.Type == DeviceAlertType.UnreadableData || (model.StatusReportId > 0 && dto.DeviceAlertId <= 0))
                {
                    dto.DeviceId = model.DeviceId;
                    dto.Type = (int)model.Type;
                    dto.InitialDeviceReportId = model.StatusReportId;
                    dto.LatestDeviceReportId = model.StatusReportId;
                    dto.ViewStatus = (int)DeviceAlertViewStatus.New;
                    dto.ResolveStatus = (int)DeviceAlertResolveStatus.New;
                    dto.InsertTS = DateTime.UtcNow;
                    dto.ResolveTS = null;
                    dto.AlertMessage = model.Message;
                }
                else
                {
                    if (model.StatusReportId > 0)
                        dto.LatestDeviceReportId = model.StatusReportId;
                    else
                    {
                        dto.ResolveStatus = (int)DeviceAlertResolveStatus.Resolved;
                        dto.ResolveTS = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}
