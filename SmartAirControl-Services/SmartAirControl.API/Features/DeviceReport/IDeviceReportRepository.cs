using SmartAirControl.API.Features.Device;
using SmartAirControl.Models.Device;
using System.Collections.Generic;

namespace SmartAirControl.API.Features.DeviceReport
{
    public interface IDeviceReportRepository
    {
        /// <summary>
        /// Saves a device report using a <see cref="DeviceReportDTO"/> instance.
        /// </summary>
        /// <param name="dto">Instance with the device health status report.</param>
        /// <returns>Id generated during the process.</returns>
        int SaveDeviceReport(DeviceReportDTO dto);

        /// <summary>
        /// Gets total of device report using a <see cref="DeviceReportTypeRegistrationIdKey"/> key.
        /// </summary>
        /// <param name="key">Key with the params to be used in the query.</param>
        /// <returns>Total count</returns>
        int GetDeviceReportTypeRegistrationIdCount(DeviceReportTypeRegistrationIdKey key);

        /// <summary>
        /// Gets total of device report using a <see cref="DeviceReportTypeRegistrationIdKey"/> key.
        /// </summary>
        /// <param name="key">Key with the params to be used in the query.</param>
        /// <returns>Total count</returns>
        IEnumerable<DeviceReportDTO> GetDeviceReportSensorByDateRange(DeviceReportSensorByDateRangeKey key);
    }
}
