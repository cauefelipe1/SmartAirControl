using System;

namespace SmartAirControl.Models.Device
{
    /// <summary>
    /// Defines a register device model to be used during the registration device process.
    /// </summary>
    public class RegisterDeviceInput
    {
        /// <see cref="DeviceModel.SerialNumber"/>
        public string SerialNumber { get; set; }

        /// <see cref="DeviceModel.SharedSecret"/>
        public string SharedSecret { get; set; }

        /// <summary>
        /// Device's firmware version.
        /// </summary>
        /// <example>1.0.2.9</example>
        public string FirmwareVersion { get; set; }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public RegisterDeviceInput() { }
    }

    /// <summary>
    /// Defines the device registration model when retrieved from the DB.
    /// </summary>
    public class DeviceRegistrationModel
    {
        /// <summary>
        /// Unique identifier for the device registration.
        /// </summary>
        /// <example>1</example>
        public int DeviceRegistrationId { get; set; }

        /// <summary>
        /// Unique device identifier ID for the API.
        /// </summary>
        /// <example>1</example>
        public int DeviceId { get; set; }

        /// <summary>
        /// Firmware reported by the device during the registration process.
        /// </summary>
        /// <example>1.35.2</example>
        public string FirmwareVersion { get; set; }

        /// <summary>
        /// Timestamp from when the registration occurred.
        /// </summary>
        /// <example>2021-10-02T12:47:49.134Z</example>
        public DateTime RegistrationTS { get; set; }
    }
}
