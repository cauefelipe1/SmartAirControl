using Dapper;
using SmartAirControl.API.Database;
using SmartAirControl.API.Features.Device;
using SmartAirControl.Models.Device;
using System.Collections.Generic;

namespace SmartAirControl.API.Features.DeviceReport
{
    public class DeviceReportPostgreSqlRepository : IDeviceReportRepository
    {
        private readonly DapperContext _dapperContext;

        public DeviceReportPostgreSqlRepository(DapperContext dapperContext) => _dapperContext = dapperContext;

        const string BASE_DEVICE_REPORT_SELECT = @"
            SELECT
                DeviceReportId,
                DeviceId,
                DeviceRegistrationId,
                ReportType,
                SensorType,
                SensorValue,
                HealthStatus,
                Message,
                DeviceReadTS,
                InsertTS
            FROM
                DeviceReport";

        const string LAST_DEVICE_REPORT_COUNT_BASE_SELECT = @"
            SELECT 
                COUNT(*) DeviceReportCount
            FROM
                DeviceReport";

        /// <inheritdoc cref="IDeviceReportRepository.GetDeviceReportTypeRegistrationIdCount(DeviceReportTypeRegistrationIdKey)"/>
        public int GetDeviceReportTypeRegistrationIdCount(DeviceReportTypeRegistrationIdKey key)
        {
            const string SQL = LAST_DEVICE_REPORT_COUNT_BASE_SELECT + @"
                WHERE 
                    DeviceRegistrationId = @DeviceRegistrationId AND
                    ReportType = @ReportType";

            using (var conn = _dapperContext.CreateConnection())
            {
                int count = conn.QuerySingleOrDefault<int>(SQL, key);

                return count;
            }
        }

        const string INSERT_DEVICE_REPORT = @"
            INSERT INTO DeviceReport (
                DeviceId,
                DeviceRegistrationId,
                ReportType,
                SensorType,
                SensorValue,
                HealthStatus,
                Message,
                DeviceReadTS,
                InsertTS
            )
            VALUES (
                @DeviceId,
                @DeviceRegistrationId,
                @ReportType,
                @SensorType,
                @SensorValue,
                @HealthStatus,
                @Message,
                @DeviceReadTS,
                @InsertTS
            )
            RETURNING DeviceReportId";

        /// <inheritdoc cref="IDeviceReportRepository.SaveDeviceReport(DeviceReportDTO)"/>
        public int SaveDeviceReport(DeviceReportDTO dto)
        {
            using (var conn = _dapperContext.CreateConnection())
            {
                int healthStatusReportId = conn.QuerySingle<int>(INSERT_DEVICE_REPORT, dto);

                return healthStatusReportId;
            }
        }

        /// <inheritdoc cref="IDeviceReportRepository.GetDeviceReportSensorByDateRange(DeviceReportSensorByDateRangeKey)"/>
        public IEnumerable<DeviceReportDTO> GetDeviceReportSensorByDateRange(DeviceReportSensorByDateRangeKey key)
        {
            const string SQL = BASE_DEVICE_REPORT_SELECT + @"
                WHERE 
                    DeviceId = @DeviceId AND
                    DeviceReadTS >= @StartDate AND
                    DeviceReadTS <= @EndDate";

            using (var conn = _dapperContext.CreateConnection())
            {
                var reports = conn.Query<DeviceReportDTO>(SQL, key);

                return reports;
            }
        }
    }
}
