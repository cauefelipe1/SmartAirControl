using SmartAirControl.Models.Base;
using System;

namespace SmartAirControl.Models.Device
{
    /// <summary>
    /// Defines the possible device status report types.
    /// </summary>
    public enum ReportyType
    {
        /// <summary>
        /// Sensor reading status report.
        /// </summary>
        SensorReading = 1,

        /// <summary>
        /// Health check status report.
        /// </summary>
        HealthCheck = 2,

        /// <summary>
        /// Unreadable reports.
        /// </summary>
        Unreadable = 3
    }

    /// <summary>
    /// Defines the possible device sensors reported by the device in the status report.
    /// </summary>
    public enum SensorType : byte
    {
        /// <summary>
        /// Temperature sensor.
        /// </summary>
        Temperature = 1,

        /// <summary>
        /// Humidity sensor.
        /// </summary>
        Humidity = 2,

        /// <summary>
        /// Carbon monoxide sensor.
        /// </summary>
        CarbonMonoxide = 3
    }

    /// <summary>
    /// Defines the model for a status report when retrieved from the DB.
    /// </summary>
    public class DeviceReportModel
    {
        /// <summary>
        /// Unique identifier for the status report.
        /// </summary>
        /// <example>1</example>
        public int DeviceReportId { get; set; }

        /// <inheritdoc cref="DeviceModel.DeviceId"/>
        public int DeviceId { get; set; }

        /// <inheritdoc cref="DeviceModel.DeviceId"/>
        public int DeviceRegistrationId { get; set; }

        /// <summary>
        /// Type of the report.
        /// <see cref="ReportyType"/> for more info.
        /// </summary>
        /// <example>SensorReading</example>
        public ReportyType ReportType { get; set; }

        /// <summary>
        /// Sensor which the device reported.
        /// <see cref="Device.SensorType"/> for more info.
        /// </summary>
        /// <example>Temperature</example>
        public SensorType? SensorType { get; set; }

        /// <summary>
        /// Sensor read value.
        /// </summary>
        /// <example>20.58</example>
        public double? SensorValue { get; set; }

        /// <summary>
        /// Health status reposted by the device.
        /// </summary>
        /// <example>OK</example>
        public string HealthStatus { get; set; }

        /// <summary>
        /// Message for traceability when a report is not parseable nor valid.
        /// Usually a value that was not able to parse.
        /// </summary>
        /// <example>10.5C</example>
        public string Message { get; set; }

        /// <summary>
        /// Timestamp from when the registration occurred in the device.
        /// </summary>
        /// <example>2021-10-02T12:47:49.134Z</example>
        public DateTime? DeviceReadTS { get; set; }

        /// <summary>
        /// Timestamp from when the report was sent to the API.
        /// </summary>
        /// <example>2021-10-02T12:47:49.134Z</example>
        public DateTime InsertTS { get; set; }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public DeviceReportModel() { }
    }

    /// <summary>
    /// Defines the input that the device will send when sending a report.
    /// </summary>
    public class DeviceReportInput
    {
        /// <inheritdoc cref="DeviceReportModel.ReportType"/>
        public ReportyType ReportType { get; set; }

        /// <inheritdoc cref="DeviceReportModel.SensorType"/>
        public SensorType? SensorType { get; set; }

        /// <inheritdoc cref="DeviceReportModel.SensorValue"/>
        public string SensorValue { get; set; }

        /// <inheritdoc cref="DeviceReportModel.DeviceReadTS"/>
        public DateTime? SnapshotTS { get; set; }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public DeviceReportInput() { }
    }

    /// <summary>
    /// Defines a aggregated version of Device reports.
    /// Usually by the system UI
    /// </summary>
    public class DeviceReportAggregated
    {
        /// <inheritdoc cref="DeviceReportModel.SensorType"/>
        public SensorType SensorType { get; set; }

        /// <summary>
        /// Start date/time of a specified aggregation date interval.
        /// </summary>
        /// <example>2021-10-02T12:47:49.134Z</example>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date/time of a specified aggregation date interval.
        /// </summary>
        /// <example>2021-10-02T12:47:49.134Z</example>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// First sensor value of a specified aggregation date interval.
        /// </summary>
        /// <example>10.0</example>
        public double FirstValue { get; set; }

        /// <summary>
        /// Last sensor value of a specified aggregation date interval.
        /// </summary>
        /// <example>20.0</example>
        public double LastValue { get; set; }

        /// <summary>
        /// Minimum sensor value of a specified aggregation date interval.
        /// </summary>
        /// <example>8.0</example>
        public double MinimumValue { get; set; }

        /// <summary>
        /// Average sensor value of a specified aggregation date interval.
        /// </summary>
        /// <example>15.7</example>
        public double AverageValue { get; set; }

        /// <summary>
        /// Maximum sensor value of a specified aggregation date interval.
        /// </summary>
        /// <example>25.0</example>
        public double MaximumValue { get; set; }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public DeviceReportAggregated() { }
    }

    /// <summary>
    /// Key to search <see cref="DeviceReportModel"/> by <see cref="DeviceRegistrationModel.DeviceRegistrationId"/> and <see cref="DeviceReportModel.ReportType"/>.
    /// </summary>
    public class DeviceReportTypeRegistrationIdKey : IModelKey<DeviceReportModel>
    {
        /// <inheritdoc cref="DeviceRegistrationModel.DeviceRegistrationId"/>
        public int DeviceRegistrationId { get; set; }

        /// <inheritdoc cref="DeviceReportModel.ReportType"/>
        public ReportyType ReportType { get; set; }
    }

    /// <summary>
    /// Key to search <see cref="DeviceReportModel"/> by <see cref="DeviceReportModel.DeviceId"/> and date interval.
    /// </summary>
    public class DeviceReportSensorByDateRangeKey : IModelKey<DeviceReportModel>
    {
        /// <inheritdoc cref="DeviceReportModel.DeviceId"/>
        public int DeviceId { get; set; }

        /// <summary>
        /// Start date/time of the interval.
        /// </summary>
        /// <example>2021-10-02T12:47:49.134Z</example>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Start date/time of the interval.
        /// </summary>
        /// <example>2021-10-02T12:47:49.134Z</example>
        public DateTime EndDate { get; set; }
    }
}
