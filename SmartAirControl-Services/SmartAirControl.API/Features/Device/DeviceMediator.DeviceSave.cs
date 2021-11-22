using MediatR;
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
        public class DeviceSaveRequest : IRequest<IEnumerable<int>>
        {
            public IEnumerable<DeviceModel> Models { get; }

            public DeviceSaveRequest(IEnumerable<DeviceModel> models)
            {
                Models = models;
            }
        }

        public class DeviceSaveHandler : IRequestHandler<DeviceSaveRequest, IEnumerable<int>>
        {
            private readonly IDeviceRepository _repo;

            public DeviceSaveHandler(IDeviceRepository repository)
            {
                _repo = repository;
            }

            public Task<IEnumerable<int>> Handle(DeviceSaveRequest request, CancellationToken cancellationToken) => Task.Run(() =>
            {
                IEnumerable<int> result = null;
                var recordsToBeSaved = new List<DeviceDTO>(request.Models.Count());

                foreach (var model in request.Models)
                {
                    ValidateModel(model);

                    var dto = BuildDTO(model);

                    recordsToBeSaved.Add(dto);
                }

                result = _repo.SaveDevice(recordsToBeSaved);

                return result;
            });

            public void ValidateModel(DeviceModel model)
            {
                if (model is null)
                    throw new Exception("Invalid model instance.");

                if (string.IsNullOrEmpty(model.SerialNumber))
                    throw new ArgumentNullException(nameof(model.SerialNumber), "Serial number message must be informed.");

                if (string.IsNullOrEmpty(model.SharedSecret))
                    throw new ArgumentNullException(nameof(model.SharedSecret), "Shared secret message must be informed.");
            }

            private DeviceDTO BuildDTO(DeviceModel model)
            {
                var dto = new DeviceDTO
                {
                    DeviceModel = model.ModelName,
                    SerialNumber = model.SerialNumber,
                    DeviceSecret = model.SharedSecret,
                    InsertTS = DateTime.UtcNow
                };

                return dto;
            }
        }
    }
}
