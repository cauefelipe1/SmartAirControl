using System;

namespace SmartAirControl.API.Features.Device
{
    public class DeviceDTO
    {
        public int DeviceId { get; set; }

        public string SerialNumber { get; set; }

        public string DeviceSecret { get; set; }

        public string DeviceModel { get; set; }

        public DateTime InsertTS { get; set; }
    }

    public class DeviceRegistrationDTO
    {
        public int RegistrationId { get; set; }

        public int DeviceId { get; set; }

        public DateTime RegistrationTS { get; set; }

        public string FirmwareVersion { get; set; }
    }

    public class DeviceReportDTO
    {
        public int DeviceReportId { get; set; }

        public int DeviceId { get; set; }

        public int DeviceRegistrationId { get; set; }

        public int ReportType { get; set; }

        public int? SensorType { get; set; }

        public double? SensorValue { get; set; }

        public string HealthStatus { get; set; }

        public string Message { get; set; }

        public DateTime? DeviceReadTS { get; set; }

        public DateTime InsertTS { get; set; }
    }

    public class DeviceAlertDTO
    {
        public int DeviceAlertId { get; set; }

        public int DeviceId { get; set; }

        public int Type { get; set; }

        public int InitialDeviceReportId { get; set; }

        public int LatestDeviceReportId { get; set; }

        public DateTime InsertTS { get; set; }

        public DateTime? ResolveTS { get; set; }

        public int ViewStatus { get; set; }

        public int ResolveStatus { get; set; }

        public string AlertMessage { get; set; }
    }
}
