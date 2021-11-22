using SmartAirControl.API.Features.Device;
using SmartAirControl.Models.Device;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartAirControl.Tests.Device
{
    internal class DeviceMockRepository : IDeviceRepository
    {
        public IEnumerable<DeviceDTO> GetAllDevice() => DeviceFixture.Devices;

        public DeviceDTO GetDeviceById(DeviceIdKey key) => DeviceFixture.Devices.FirstOrDefault(d => d.DeviceId == key.DeviceId);

        public DeviceDTO GetDeviceBySerialNumber(DeviceSerialNumberKey key) => DeviceFixture.Devices.FirstOrDefault(d => d.SerialNumber == key.SerialNumber);

        public IEnumerable<DeviceRegistrationDTO> GetDeviceRegistrations(int deviceId) => DeviceFixture.Registrations.Where(r => r.DeviceId == deviceId);

        public DeviceRegistrationModel GetLastDeviceRegistration(int deviceId)
        {
            int lastRegistrationId = GetLastDeviceRegistrationId(deviceId);

            var registration = DeviceFixture.Registrations.FirstOrDefault(r => r.RegistrationId == lastRegistrationId);

            return new()
            {
                DeviceRegistrationId = registration.RegistrationId,
                DeviceId = registration.DeviceId,
                FirmwareVersion = registration.FirmwareVersion,
                RegistrationTS = registration.RegistrationTS
            };
        }

        public int GetLastDeviceRegistrationId(int deviceId) => DeviceFixture.Registrations.Where(r => r.DeviceId == deviceId).Max(r => r.RegistrationId);

        public IEnumerable<int> SaveDevice(IEnumerable<DeviceDTO> dtos)
        {
            int lastDeviceId = DeviceFixture.Devices.Max(d => d.DeviceId);
            var result = new List<int>(dtos.Count());
            
            foreach (var dto in dtos)
            {
                dto.DeviceId = ++lastDeviceId;
                
                DeviceFixture.Devices.Add(dto);
                
                result.Add(dto.DeviceId);
            }
            
            return result;
        }

        public int SaveDeviceRegistration(DeviceRegistrationDTO dto)
        {
            int lastId = DeviceFixture.Registrations.Max(d => d.RegistrationId);
            dto.RegistrationId = ++lastId;
            DeviceFixture.Registrations.Add(dto);

            return dto.RegistrationId;
        }
    }
}
