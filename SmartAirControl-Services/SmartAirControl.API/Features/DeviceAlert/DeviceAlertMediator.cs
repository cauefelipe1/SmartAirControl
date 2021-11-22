using MediatR;
using SmartAirControl.API.Features.Device;
using SmartAirControl.Models.Base;
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
        public class DeviceAlertQueryRequest : IRequest<IEnumerable<DeviceAlertModel>>
        {
            public IModelKey<DeviceAlertModel> Key { get; }

            public DeviceAlertQueryRequest(IModelKey<DeviceAlertModel> key)
            {
                Key = key;
            }
        }

        public class DeviceAlertQueryHandler : IRequestHandler<DeviceAlertQueryRequest, IEnumerable<DeviceAlertModel>>
        {
            private readonly IDeviceAlertRepository _repo;

            public DeviceAlertQueryHandler(IDeviceAlertRepository repository)
            {
                _repo = repository;
            }

            public Task<IEnumerable<DeviceAlertModel>> Handle(DeviceAlertQueryRequest request, CancellationToken cancellationToken) => Task.Run(() =>
            {
                IEnumerable<DeviceAlertModel> result = null;

                if (request.Key is DeviceAlertIdKey idKey)
                {
                    var dto = _repo.GetDeviceAlertById(idKey);

                    if (dto is not null)
                    {
                        result = new List<DeviceAlertModel>
                        {
                            BuildModel(dto)
                        };
                    }
                }
                else if (request.Key is DeviceAlertTypeResolveStatusKey typeResolveStatusKey)
                {
                    var dtos = _repo.GetDeviceAlertTypeResolveStatus(typeResolveStatusKey);

                    result = dtos.Select(dto => BuildModel(dto));
                }
                else if (request.Key is DeviceAlertResolveViewStatusKey typeResolveViewStatus)
                {
                    var dtos = _repo.GetDeviceAlertResolveViewStatus(typeResolveViewStatus);

                    result = dtos.Select(dto => BuildModel(dto));
                }
                else
                {
                    throw new Exception($"Missing an implementation for {request.Key.GetType()}");
                }

                return result;
            });

            private DeviceAlertModel BuildModel(DeviceAlertDTO dto)
            {
                var model = new DeviceAlertModel
                {
                    DeviceAlertId = dto.DeviceAlertId,
                    DeviceId = dto.DeviceId,
                    AlertType = (DeviceAlertType)dto.Type,
                    InitialDeviceReportId = dto.InitialDeviceReportId,
                    LatestDeviceReportId = dto.LatestDeviceReportId,
                    Message = dto.AlertMessage,
                    VisualizationStatus = (DeviceAlertViewStatus)dto.ViewStatus,
                    ResolveStatus = (DeviceAlertResolveStatus)dto.ResolveStatus,
                    InsertTimestamp = dto.InsertTS,
                    ResolveTimestamp = dto.ResolveTS
                };

                return model;

            }
        }
    }
}
