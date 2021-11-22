using SmartAirControl.Models.Device;
using Xunit;
using System.Threading;
using System.Linq;
using SmartAirControl.Tests.Jwt;
using MediatR;
using Moq;
using System.Threading.Tasks;
using static SmartAirControl.API.Features.Device.DeviceMediator;

namespace SmartAirControl.Tests.Device
{
    public class DeviceTests : IClassFixture<DeviceFixture>
    {
        [Fact(DisplayName = "Inserts a new device")]
        public async Task InsertNewDevice()
        {
            var newDevice = new DeviceModel()
            {
                ModelName = "Mark_II",
                SerialNumber = "vnjG25Hvb84FQf4c",
                SharedSecret = "Dx<c*e{AHE32T7!7H[?!@s,-PKUR>'j(tJJb_>~Mg?8z{'mWZt(6`{W"
            };

            var queryParam = new DeviceSaveRequest(new[] { newDevice });
            var mediator = new DeviceSaveHandler(new DeviceMockRepository());
            
            await mediator.Handle(queryParam, new CancellationToken());

            Assert.Contains(DeviceFixture.Devices, d => string.Equals(d.SerialNumber, newDevice.SerialNumber));
        }

        [Fact(DisplayName = "Gets a device")]
        public async Task GetDevice()
        {
            var deviceDto = DeviceFixture.Devices.First();

            var queryParam = new DeviceQueryRequest(new DeviceIdKey { DeviceId = deviceDto.DeviceId});
            var mediator = new DeviceQueryRequestHandler(new DeviceMockRepository());

            var deviceModels = await mediator.Handle(queryParam, new CancellationToken());

            Assert.Single(deviceModels);

            Assert.Equal(deviceDto.DeviceId, deviceModels.ElementAt(0).DeviceId);
        }

        [Fact(DisplayName = "Device registrates itself.")]
        public async Task DeviceRegistratesItself()
        {
            var deviceDto = DeviceFixture.Devices.First();

            var queryParam = new DeviceQueryRequest(new DeviceIdKey { DeviceId = deviceDto.DeviceId });
            var queryMediator = new DeviceQueryRequestHandler(new DeviceMockRepository());

            var modelBefore = (await queryMediator.Handle(queryParam, new CancellationToken())).ElementAt(0);

            var registrationInput = new RegisterDeviceInput
            {
                SerialNumber = deviceDto.SerialNumber,
                SharedSecret = deviceDto.DeviceSecret,
                FirmwareVersion = "FM_TEST_V1"
            };
            
            var mockMediator = new Mock<IMediator>();

            mockMediator
                .Setup(m => m.Send(It.IsAny<DeviceQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { modelBefore });

            var registerParam = new DeviceRegistrationRequest(registrationInput);
            var registerMediator = new DeviceRegistrationHandler(new DeviceMockRepository(), new MockJwtService(), mockMediator.Object);

            var tokenInfo = await registerMediator.Handle(registerParam, new CancellationToken());
            var modelAfter = (await queryMediator.Handle(queryParam, new CancellationToken())).ElementAt(0);

            Assert.NotNull(tokenInfo);
            Assert.Equal(modelBefore.Registrations.Count() + 1, modelAfter.Registrations.Count());
        }
    }
}