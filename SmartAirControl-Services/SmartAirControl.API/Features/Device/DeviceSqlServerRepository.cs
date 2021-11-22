using Dapper;
using SmartAirControl.API.Database;
using SmartAirControl.Models.Device;
using System.Collections.Generic;
using System.Linq;

namespace SmartAirControl.API.Features.Device
{
    public class DeviceSqlServerRepository : IDeviceRepository
    {
        private readonly DapperContext _dapperContext;

        public DeviceSqlServerRepository(DapperContext dapperContext) => _dapperContext = dapperContext;

        const string SELECT_DEVICE_BASE_SQL = @"
            SELECT 
                DeviceId,
                SerialNumber,
                DeviceSecret,
                DeviceModel,
                InsertTS
            FROM
                Device";

        /// <inheritdoc cref="IDeviceRepository.GetDeviceById(DeviceIdKey)"/>
        public DeviceDTO GetDeviceById(DeviceIdKey key)
        {
            const string SQL = SELECT_DEVICE_BASE_SQL + @"
                WHERE 
                    DeviceId = @DeviceId";

            using (var conn = _dapperContext.CreateConnection())
            {
                var deviceDto = conn.QuerySingleOrDefault<DeviceDTO>(SQL, key);

                return deviceDto;
            }
        }

        /// <inheritdoc cref="IDeviceRepository.GetDeviceBySerialNumber(DeviceSerialNumberKey)"/>
        public DeviceDTO GetDeviceBySerialNumber(DeviceSerialNumberKey key)
        {
            const string SQL = SELECT_DEVICE_BASE_SQL + @"
                WHERE 
                    SerialNumber = @SerialNumber";

            using (var conn = _dapperContext.CreateConnection())
            {
                var deviceDto = conn.QuerySingleOrDefault<DeviceDTO>(SQL, key);

                return deviceDto;
            }
        }

        const string BASE_DEVICE_REGISTRATION_SELECT = @"
            SELECT
                DeviceRegistrationId,
                DeviceId,
                FirmwareVersion,
                RegistrationTS
            FROM
                DeviceRegistration";

        /// <inheritdoc cref="IDeviceRepository.GetLastDeviceRegistration(int)"/>
        public DeviceRegistrationModel GetLastDeviceRegistration(int deviceId)
        {
            const string SQL = BASE_DEVICE_REGISTRATION_SELECT + @"
                WHERE 
                    RegistrationAttemptId = (" + LAST_DEVICE_REGISTRATION_ID + ")";

            using (var conn = _dapperContext.CreateConnection())
            {
                var registration = conn.QuerySingleOrDefault<DeviceRegistrationModel>(SQL, new { deviceId });

                return registration;
            }
        }

        const string INSERT_DEVICE_REGISTRATION = @"
            INSERT INTO DeviceRegistration (
                DeviceId,
                FirmwareVersion,
                RegistrationTS
            ) 
            OUTPUT INSERTED.DeviceRegistrationId
            VALUES (
                @DeviceId,
                @FirmwareVersion,
                @RegistrationTS
            )";

        /// <inheritdoc cref="IDeviceRepository.SaveDeviceRegistration(DeviceRegistrationDTO)"/>
        public int SaveDeviceRegistration(DeviceRegistrationDTO dto)
        {
            using (var conn = _dapperContext.CreateConnection())
            {
                int registrationAttemptId = conn.QuerySingle<int>(INSERT_DEVICE_REGISTRATION, dto);

                return registrationAttemptId;
            }
        }

        const string LAST_DEVICE_REGISTRATION_ID = @"
            SELECT 
                COALESCE(MAX(RegistrationAttemptId), 0) RegistrationId
            FROM
                DeviceRegistration
            WHERE
                DeviceID = @DeviceId";

        /// <inheritdoc cref="IDeviceRepository.GetLastDeviceRegistrationId(int)"/>
        public int GetLastDeviceRegistrationId(int deviceId)
        {
            using (var conn = _dapperContext.CreateConnection())
            {
                int registrationId = conn.QuerySingleOrDefault<int>(LAST_DEVICE_REGISTRATION_ID, new { deviceId });

                return registrationId;
            }
        }

        const string INSERT_DEVICE = @"
            INSERT INTO Device (
                SerialNumber,
                DeviceSecret,
                DeviceModel,
                InsertTS
            ) 
            OUTPUT.DeviceId
            VALUES (
                @SerialNumber,
                @DeviceSecret,
                @DeviceModel,
                @InsertTS
            )";

        /// <inheritdoc cref="IDeviceRepository.SaveDevice(IEnumerable{DeviceDTO})"/>
        public IEnumerable<int> SaveDevice(IEnumerable<DeviceDTO> dtos)
        {
            var result = new List<int>(dtos.Count());

            using (var conn = _dapperContext.CreateConnection())
            {
                foreach (var dto in dtos)
                {
                    int deviceId = conn.QuerySingle<int>(INSERT_DEVICE, dto);

                    result.Add(deviceId);
                }
            }

            return result;
        }

        public IEnumerable<DeviceRegistrationDTO> GetDeviceRegistrations(int deviceId)
        {
            const string SQL = BASE_DEVICE_REGISTRATION_SELECT + @"
                WHERE 
                    DeviceId = @DeviceId
                ORDER BY
	                DeviceID DESC";

            using (var conn = _dapperContext.CreateConnection())
            {
                var registrations = conn.Query<DeviceRegistrationDTO>(SQL, new { deviceId });

                return registrations;
            }
        }

        /// <inheritdoc cref="IDeviceRepository.GetAllDevice"/>
        public IEnumerable<DeviceDTO> GetAllDevice()
        {
            using (var conn = _dapperContext.CreateConnection())
            {
                var devices = conn.Query<DeviceDTO>(SELECT_DEVICE_BASE_SQL);

                return devices;
            }
        }
    }
}
