using MediatR;
using SmartAirControl.API.Core.Jwt;
using SmartAirControl.API.Features.DeviceAlert;
using SmartAirControl.Models.Authentication;
using SmartAirControl.Models.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAirControl.API.Features.Device
{
    public partial class DeviceReportMediator
    {
        public class DeviceRegistrationRequest : IRequest<TokenInfo>
        {
            public RegisterDeviceInput Model { get; }

            public DeviceRegistrationRequest(RegisterDeviceInput model)
            {
                Model = model;
            }
        }

        public class DeviceRegistrationHandler : IRequestHandler<DeviceRegistrationRequest, TokenInfo>
        {
            private readonly IDeviceRepository _repo;
            private readonly IJwtService _jwtService;
            private readonly IMediator _mediator;

            public DeviceRegistrationHandler(IDeviceRepository repository, IJwtService jwtService, IMediator mediator)
            {
                _repo = repository;
                _jwtService = jwtService;
                _mediator = mediator;
            }

            public async Task<TokenInfo> Handle(DeviceRegistrationRequest request, CancellationToken cancellationToken)
            {
                ValidateModel(request.Model);

                var deviceKey = new DeviceSerialNumberKey { SerialNumber = request.Model.SerialNumber };
                var deviceModels = await _mediator.Send(new DeviceMediator.DeviceQueryRequest(deviceKey));
                var deviceModel = deviceModels?.FirstOrDefault();

                if (deviceModel is null)
                {
                    deviceModel = BuildDeviceModel(request.Model);
                    var deviceIds = await _mediator.Send(new DeviceMediator.DeviceSaveRequest(new[] { deviceModel }));

                    deviceModel.DeviceId = deviceIds.FirstOrDefault();
                }

                var key = new DeviceAlertTypeResolveStatusKey
                {
                    DeviceId = deviceModel.DeviceId,
                    AlertType = DeviceAlertType.UnreadableData,
                    ResolveStatus = DeviceAlertResolveStatus.New
                };

                var alertTask = _mediator.Send(new DeviceAlertMediator.DeviceAlertQueryRequest(key));

                var dto = BuildDTO(request.Model, deviceModel.DeviceId);

                int registrationAttemptId = _repo.SaveDeviceRegistration(dto);

                var registrationModel = new DeviceRegistrationModel
                {
                    DeviceRegistrationId = registrationAttemptId,
                    DeviceId = dto.DeviceId,
                    FirmwareVersion = dto.FirmwareVersion,
                    RegistrationTS = dto.RegistrationTS
                };

                var deviceAlerts = await alertTask;
                var deviceAlert = deviceAlerts.FirstOrDefault();

                if (deviceAlert is not null)
                {
                    deviceAlert.ResolveStatus = DeviceAlertResolveStatus.Resolved;
                    deviceAlert.ResolveTimestamp = DateTime.UtcNow;

                    await _mediator.Send(new DeviceAlertMediator.DeviceAlertUpdateRequest(new[] { deviceAlert }));
                }

                var devicePayload = GetDeviceTokenPayload(registrationModel);
                var tokenInfo = _jwtService.GenerateJwtToken(devicePayload);

                return tokenInfo;
            }

            private Dictionary<string, string> GetDeviceTokenPayload(DeviceRegistrationModel registrationModel)
            {
                var payload = new Dictionary<string, string>
                {
                    [nameof(IdentityClaimsModel.UserId)] = registrationModel.DeviceId.ToString(),
                    [nameof(IdentityClaimsModel.DeviceRegistrationId)] = registrationModel.DeviceRegistrationId.ToString(),
                    [nameof(IdentityClaimsModel.DeviceFirmwareVersion)] = registrationModel.FirmwareVersion
                };

                return payload;
            }

            public void ValidateModel(RegisterDeviceInput model)
            {
                if (model is null)
                    throw new Exception("Invalid model instance.");

                if (string.IsNullOrEmpty(model.SharedSecret))
                    throw new ArgumentNullException(nameof(model.SharedSecret), "The shared secret must be informed.");

                if (string.IsNullOrEmpty(model.FirmwareVersion))
                    throw new ArgumentNullException(nameof(model.FirmwareVersion), "The firmware version must be informed.");
            }

            private DeviceRegistrationDTO BuildDTO(RegisterDeviceInput model, int deviceId)
            {
                var dto = new DeviceRegistrationDTO
                {
                    DeviceId = deviceId,
                    FirmwareVersion = model.FirmwareVersion,
                    RegistrationTS = DateTime.UtcNow
                };

                return dto;
            }

            private DeviceModel BuildDeviceModel(RegisterDeviceInput input)
            {
                var model = new DeviceModel
                {
                    SerialNumber = input.SerialNumber,
                    SharedSecret = input.SharedSecret,
                    ModelName = "SmartAC_prototype"
                };

                return model;
            }
        }
    }
}
