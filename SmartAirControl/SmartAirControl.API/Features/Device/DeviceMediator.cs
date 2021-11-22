using MediatR;
using SmartAirControl.Models.Base;
using SmartAirControl.Models.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAirControl.API.Features.Device
{
    public partial class DeviceMediator
    {
        public class DeviceQueryRequest : IRequest<IEnumerable<DeviceModel>>
        {
            public IModelKey<DeviceModel> Key { get; }

            public DeviceQueryRequest(IModelKey<DeviceModel> key)
            {
                Key = key;
            }
        }

        public class DeviceQueryRequestHandler : IRequestHandler<DeviceQueryRequest, IEnumerable<DeviceModel>>
        {
            private readonly IDeviceRepository _repo;

            public DeviceQueryRequestHandler(IDeviceRepository repository)
            {
                _repo = repository;
            }

            public Task<IEnumerable<DeviceModel>> Handle(DeviceQueryRequest request, CancellationToken cancellationToken) => Task.Run(() =>
            {
                IEnumerable<DeviceModel> result = null;

                if (request.Key is DeviceIdKey idKey)
                {
                    var dto = _repo.GetDeviceById(idKey);

                    if (dto is not null)
                    {
                        result = new List<DeviceModel>
                        {
                            BuildModel(dto)
                        };
                    }
                }
                else if (request.Key is DeviceSerialNumberKey serialKey)
                {
                    var dto = _repo.GetDeviceBySerialNumber(serialKey);

                    if (dto is not null)
                    {
                        result = new List<DeviceModel>
                        {
                            BuildModel(dto)
                        };
                    }
                }
                else if (request.Key is DeviceAllKey allKey)
                {
                    var dtos = _repo.GetAllDevice();

                    result = dtos.Select(dto => BuildModel(dto));
                }
                else
                {
                    throw new Exception($"Missing an implementation for {request.Key.GetType()}");
                }

                return result;
            });

            private void AssignRegistrations(DeviceModel model)
            {
                var registrations = _repo.GetDeviceRegistrations(model.DeviceId);

                model.Registrations.AddRange(registrations.Select(dto => BuildRegistrationModel(dto)));
            }

            private DeviceRegistrationModel BuildRegistrationModel(DeviceRegistrationDTO dto)
            {
                var model = new DeviceRegistrationModel
                {
                    DeviceRegistrationId = dto.RegistrationId,
                    DeviceId = dto.DeviceId,
                    FirmwareVersion = dto.FirmwareVersion,
                    RegistrationTS = dto.RegistrationTS
                };

                return model;
            }

            private DeviceModel BuildModel(DeviceDTO dto)
            {
                var model = new DeviceModel
                {
                    DeviceId = dto.DeviceId,
                    SerialNumber = dto.SerialNumber,
                    ModelName = dto.DeviceModel,
                    SharedSecret = new string('*', dto.DeviceSecret.Length)
                };

                AssignRegistrations(model);

                return model;
            }
        }
    }
}
