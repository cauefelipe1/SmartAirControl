using SmartAirControl.Models.Device;
using System.Collections.Generic;

namespace SmartAirControl.API.Features.Device
{
    public interface IDeviceRepository
    {
        /// <summary>
        /// Gets a list of devices using <see cref="DeviceIdKey"/>
        /// </summary>
        /// <param name="key">KEy with the params.</param>
        DeviceDTO GetDeviceById(DeviceIdKey key);

        /// <summary>
        /// Gets a list of devices using <see cref="DeviceSerialNumberKey"/>
        /// </summary>
        /// <param name="key">KEy with the params.</param>
        DeviceDTO GetDeviceBySerialNumber(DeviceSerialNumberKey key);

        /// <summary>
        /// Gets a list of devices all devices.
        /// </summary>
        IEnumerable<DeviceDTO> GetAllDevice();

        /// <summary>
        /// Saves a device regitration using a <see cref="DeviceRegistrationDTO"/> instance.
        /// </summary>
        /// <param name="dto">Instance with the device register info.</param>
        /// <returns>Id generated during the process.</returns>
        int SaveDeviceRegistration(DeviceRegistrationDTO dto);

        /// <summary>
        /// Gets the id of the last registration record for for a device.
        /// </summary>
        /// <param name="deviceId">Device's id.</param>
        /// <returns>Id of the last device registration.</returns>
        int GetLastDeviceRegistrationId(int deviceId);

        /// <summary>
        /// Gets the last registration record for for a device.
        /// </summary>
        /// <param name="deviceId">Device's id.</param>
        /// <returns>Instance of the last device registration.</returns>
        DeviceRegistrationModel GetLastDeviceRegistration(int deviceId);

        /// <summary>
        /// Gets all registrations of a device.
        /// </summary>
        /// <param name="deviceId">Device's id to fetch the registrations.</param>
        /// <returns>List of device registrations.</returns>
        IEnumerable<DeviceRegistrationDTO> GetDeviceRegistrations(int deviceId);

        /// <summary>
        /// Update a list of <see cref="DeviceAlertDTO"/> DTOs.
        /// </summary>
        /// <param name="dtos">List with DTOs to be persisted</param>
        IEnumerable<int> SaveDevice(IEnumerable<DeviceDTO> dtos);
    }
}
