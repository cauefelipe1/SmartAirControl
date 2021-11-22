using SmartAirControl.API.Features.Device;
using SmartAirControl.Models.Device;
using System.Collections.Generic;

namespace SmartAirControl.API.Features.DeviceAlert
{
    public interface IDeviceAlertRepository
    {
        /// <summary>
        /// Gets a device alert using a <see cref="DeviceAlertIdKey"/> key.
        /// </summary>
        /// <param name="key">Key with the params to be used in the query.</param>
        /// <returns>List of DTOs instance</returns>
        DeviceAlertDTO GetDeviceAlertById(DeviceAlertIdKey key);

        /// <summary>
        /// Gets a list of device alert using a <see cref="DeviceAlertTypeResolveStatusKey"/> key.
        /// </summary>
        /// <param name="key">Key with the params to be used in the query.</param>
        /// <returns>List of DTOs instance</returns>
        List<DeviceAlertDTO> GetDeviceAlertTypeResolveStatus(DeviceAlertTypeResolveStatusKey key);

        /// <summary>
        /// Gets a list of device alert using a <see cref="DeviceAlertResolveViewStatusKey"/> key.
        /// </summary>
        /// <param name="key">Key with the params to be used in the query.</param>
        /// <returns>List of DTOs instance</returns>
        List<DeviceAlertDTO> GetDeviceAlertResolveViewStatus(DeviceAlertResolveViewStatusKey key);

        /// <summary>
        /// Saves a device health status report using a instance of <see cref="DeviceAlertDTO"/> key.
        /// </summary>
        /// <param name="dto">Info to be persisted.</param>
        void SaveDeviceAlert(DeviceAlertDTO dto);

        /// <summary>
        /// Update a list of <see cref="DeviceAlertDTO"/> DTOs.
        /// </summary>
        /// <param name="dtos">List with DTOs to be persisted</param>
        void UpdateDeviceAlert(List<DeviceAlertDTO> dtos);
    }
}
