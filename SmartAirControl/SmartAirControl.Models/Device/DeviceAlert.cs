using SmartAirControl.Models.Base;
using System;
using System.Collections.Generic;

namespace SmartAirControl.Models.Device
{
    /// <summary>
    /// Defines the possible type of device alerts.
    /// </summary>
    public enum DeviceAlertType : byte
    {
        /// <summary>
        /// Temperature out of range.
        /// </summary>
        TemperatureRange = 1,

        /// <summary>
        /// Humidity out of range.
        /// </summary>
        HumidtyRange = 2,

        /// <summary>
        /// Carbon monoxide out of range.
        /// </summary>
        CarbonRange = 3,

        /// <summary>
        /// Carbon monoxide in dangerous level.
        /// </summary>
        CarbonDanger = 4,

        /// <summary>
        /// Device reported a health problem.
        /// </summary>
        HealthProblem = 5,

        /// <summary>
        /// Device send too many unreadable reports.
        /// </summary>
        UnreadableData = 6
    }

    /// <summary>
    /// Defines the possible view statuses of a device alert.
    /// </summary>
    public enum DeviceAlertViewStatus : byte
    {
        /// <summary>
        /// Alert was created and not view yet.
        /// </summary>
        New = 1,

        /// <summary>
        /// Alert has been visualized.
        /// </summary>
        Viewd = 2
    }

    /// <summary>
    /// Defines the possible resolve statuses of a device alert.
    /// </summary>
    public enum DeviceAlertResolveStatus : byte
    {
        /// <summary>
        /// Alert was created and not view yet.
        /// </summary>
        New = 1,

        /// <summary>
        /// Alert has been resolved.
        /// </summary>
        Resolved = 2,

        /// <summary>
        /// Alert was ignored by the user.
        /// </summary>
        Ignored = 3
    }

    /// <summary>
    /// Defines the model for a status report when retrieved from the DB.
    /// </summary>
    public class DeviceAlertModel
    {
        /// <summary>
        /// Unique identifier for the device alert.
        /// </summary>
        /// <example>1</example>
        public int DeviceAlertId { get; set; }

        /// <inheritdoc cref="DeviceModel.DeviceId"/>
        public int DeviceId { get; set; }

        /// <summary>
        /// Type of the device alert.
        /// <see cref="DeviceAlertType"/> for more info.
        /// </summary>
        /// <example>TemperatureRange</example>
        public DeviceAlertType AlertType { get; set; }

        /// <summary>
        /// <see cref="DeviceReportModel"/> that has started the alert.
        /// <see cref="DeviceReportModel"/> for more info.
        /// </summary>
        /// <example>1</example>
        public int InitialDeviceReportId { get; set; }

        /// <summary>
        /// Last known <see cref="DeviceReportModel"/> which reported the alert.
        /// <see cref="DeviceReportModel"/> for more info.
        /// </summary>
        /// <example>10</example>
        public int LatestDeviceReportId { get; set; }

        /// <summary>
        /// Timestamp from when the alert was created.
        /// </summary>
        /// <example>2021-10-02T12:47:49.134Z</example>
        public DateTime InsertTimestamp { get; set; }

        /// <summary>
        /// Timestamp from when the alert was resolved.
        /// </summary>
        /// <example>2021-10-02T12:47:49.134Z</example>
        public DateTime? ResolveTimestamp { get; set; }

        /// <summary>
        /// View status of the alert.
        /// <see cref="DeviceAlertViewStatus"/> for more info.
        /// </summary>
        /// <example>View</example>
        public DeviceAlertViewStatus VisualizationStatus { get; set; }

        /// <summary>
        /// Resolve status of the alert.
        /// <see cref="DeviceAlertResolveStatus"/> for more info.
        /// </summary>
        /// <example>Ignored</example>
        public DeviceAlertResolveStatus ResolveStatus { get; set; }

        /// <summary>
        /// Message which detail of the alert.
        /// </summary>
        /// <example>Device is reporting health problem</example>
        public string Message { get; set; }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public DeviceAlertModel() { }
    }


    /// <summary>
    /// Defines the input for creating a new alert.
    /// </summary>
    public class DeviceAlertSaveInput
    {
        /// <inheritdoc cref="DeviceModel.DeviceId"/>
        public int DeviceId { get; set; }

        /// <inheritdoc cref="DeviceAlertModel.AlertType"/>
        public DeviceAlertType Type { get; set; }

        /// <inheritdoc cref="DeviceAlertModel.Message"/>
        public string Message { get; set; }

        /// <summary>
        /// When greater than 0 indicates the status report create the alert.
        /// When zero indicates that the last alert (if available) can be marked as resolved.
        /// </summary>
        public int StatusReportId { get; set; }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public DeviceAlertSaveInput() { }
    }

    /// <summary>
    /// Key to search <see cref="DeviceAlertModel"/> by <see cref="DeviceAlertModel.DeviceId"/>, 
    /// <see cref="DeviceAlertModel.AlertType"/> and <see cref="DeviceAlertModel.ResolveStatus"/>.
    /// </summary>
    public class DeviceAlertTypeResolveStatusKey : IModelKey<DeviceAlertModel>
    {
        /// <inheritdoc cref="DeviceModel.DeviceId"/>
        public int DeviceId { get; set; }

        /// <inheritdoc cref="DeviceAlertModel.AlertType"/>
        public DeviceAlertType AlertType { get; set; }

        /// <inheritdoc cref="DeviceAlertModel.ResolveStatus"/>
        public DeviceAlertResolveStatus ResolveStatus { get; set; }
    }

    /// <summary>
    /// Key to search <see cref="DeviceAlertModel"/> by a list of <see cref="DeviceAlertViewStatus"/> and a list of <see cref="DeviceAlertResolveStatus"/>.
    /// </summary>
    public class DeviceAlertResolveViewStatusKey : IModelKey<DeviceAlertModel>
    {
        /// <inheritdoc cref="DeviceAlertModel.AlertType"/>
        public IEnumerable<DeviceAlertViewStatus> ViewStatus { get; set; }

        /// <inheritdoc cref="DeviceAlertModel.ResolveStatus"/>
        public IEnumerable<DeviceAlertResolveStatus> ResolveStatus { get; set; }
    }

    /// <summary>
    /// Key to search <see cref="DeviceAlertModel"/> by a <see cref="DeviceAlertModel.DeviceAlertId"/>.
    /// </summary>
    public class DeviceAlertIdKey : IModelKey<DeviceAlertModel>
    {
        /// <inheritdoc cref="DeviceAlertModel.DeviceAlertId"/>
        public int DeviceAlertId { get; set; }
    }
}
