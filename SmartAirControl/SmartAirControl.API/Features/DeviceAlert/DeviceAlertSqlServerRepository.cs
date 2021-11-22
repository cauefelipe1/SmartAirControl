using Dapper;
using SmartAirControl.API.Database;
using SmartAirControl.API.Features.Device;
using SmartAirControl.Models.Device;
using System.Collections.Generic;
using System.Linq;

namespace SmartAirControl.API.Features.DeviceAlert
{

    public class DeviceAlertSqlServerRepository : IDeviceAlertRepository
    {
        private readonly DapperContext _dapperContext;

        public DeviceAlertSqlServerRepository(DapperContext dapperContext) => _dapperContext = dapperContext;

        const string BASE_DEVICE_ALERT_SELECT = @"
            SELECT
                DeviceAlertId,
                DeviceId,
                Type,
                InitialDeviceReportId,
                LatestDeviceReportId,
                InsertTS,
                ResolveTS,
                ViewStatus,
                ResolveStatus,
                AlertMessage
            FROM
                DeviceAlert";

        /// <inheritdoc cref="IDeviceAlertRepository.GetDeviceAlertTypeResolveStatus(DeviceAlertTypeResolveStatusKey)"/>
        public List<DeviceAlertDTO> GetDeviceAlertTypeResolveStatus(DeviceAlertTypeResolveStatusKey key)
        {
            const string SQL = BASE_DEVICE_ALERT_SELECT + @"
                WHERE 
                    DeviceId = @DeviceId AND 
                    Type = @AlertType AND
                    ResolveStatus = @ResolveStatus";

            using (var conn = _dapperContext.CreateConnection())
            {
                var reportAlerts = conn.Query<DeviceAlertDTO>(SQL, key).ToList();

                return reportAlerts;
            }
        }

        const string INSERT_DEVICE_ALERT = @"
            INSERT INTO DeviceAlert (
                DeviceId,
                Type,
                InitialDeviceReportId,
                LatestDeviceReportId,
                InsertTS,
                ResolveTS,
                ViewStatus,
                ResolveStatus,
                AlertMessage
            )
            VALUES (
                @DeviceId,
                @Type,
                @InitialDeviceReportId,
                @LatestDeviceReportId,
                @InsertTS,
                @ResolveTS,
                @ViewStatus,
                @ResolveStatus,
                @AlertMessage
            )";

        const string UPDATE_DEVICE_ALERT_SQL = @"
            UPDATE
                DeviceAlert
            SET
                LatestDeviceReportId = @LatestDeviceReportId,
                ResolveTS = @ResolveTS,
                ViewStatus = @ViewStatus,
                ResolveStatus = @ResolveStatus
            WHERE
                DeviceAlertId = @DeviceAlertId";

        /// <inheritdoc cref="IDeviceAlertRepository.SaveDeviceAlert(DeviceAlertDTO)"/>
        public void SaveDeviceAlert(DeviceAlertDTO dto)
        {
            using (var conn = _dapperContext.CreateConnection())
            {
                if (dto.DeviceAlertId <= 0)
                    conn.Execute(INSERT_DEVICE_ALERT, dto);
                else
                    conn.Execute(UPDATE_DEVICE_ALERT_SQL, dto);
            }
        }

        /// <inheritdoc cref="IDeviceAlertRepository.UpdateDeviceAlert(List{DeviceAlertDTO})"/>
        public void UpdateDeviceAlert(List<DeviceAlertDTO> dtos)
        {
            using (var conn = _dapperContext.CreateConnection())
            {
                conn.Execute(UPDATE_DEVICE_ALERT_SQL, dtos);
            }
        }

        /// <inheritdoc cref="IDeviceAlertRepository.GetDeviceAlertResolveViewStatus(DeviceAlertResolveViewStatusKey)"/>
        public List<DeviceAlertDTO> GetDeviceAlertResolveViewStatus(DeviceAlertResolveViewStatusKey key)
        {
            const string SQL = BASE_DEVICE_ALERT_SELECT + @"
                WHERE 
                    ResolveStatus IN @ResolveStatus AND
                    ViewStatus IN @ViewStatus";

            var param = new
            {
                ResolveStatus = key.ResolveStatus.Select(r => (int)r),
                ViewStatus = key.ViewStatus.Select(v => (int)v),
            };

            using (var conn = _dapperContext.CreateConnection())
            {
                var reportAlerts = conn.Query<DeviceAlertDTO>(SQL, param).ToList();

                return reportAlerts;
            }
        }

        /// <inheritdoc cref="IDeviceAlertRepository.GetDeviceAlertById(DeviceAlertIdKey)"/>
        public DeviceAlertDTO GetDeviceAlertById(DeviceAlertIdKey key)
        {
            const string SQL = BASE_DEVICE_ALERT_SELECT + @"
                WHERE 
                    DeviceAlertId = @DeviceAlertId";

            using (var conn = _dapperContext.CreateConnection())
            {
                var reportAlert = conn.QuerySingleOrDefault<DeviceAlertDTO>(SQL, key);

                return reportAlert;
            }
        }
    }
}
