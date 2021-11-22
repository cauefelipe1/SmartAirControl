using SmartAirControl.Models.Base;
using System;
using System.Collections.Generic;

namespace SmartAirControl.Models.Device
{
    /// <summary>
    /// Defines the model for device.
    /// </summary>
    public class DeviceModel
    {
        /// <summary>
        /// Device's ID inside the system.
        /// </summary>
        /// <example>1</example>
        public int DeviceId { get; set; }

        /// <summary>
        /// Device's serial number.
        /// </summary>
        /// <example>567s87ds32fdawe54fd54sdfsd545</example>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Device's model name.
        /// </summary>
        /// <example>SmartAC_prototype</example>
        public string ModelName { get; set; }

        /// <summary>
        /// Device's shared secret.
        /// </summary>
        /// <example>33sd455fd</example>
        public string SharedSecret { get; set; }

        /// <summary>
        /// List of all registrations for the device.
        /// </summary>
        /// <example>33sd455fd</example>
        public List<DeviceRegistrationModel> Registrations { get; set; }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public DeviceModel()
        {
            Registrations = new();
        }
    }

    /// <summary>
    /// Defines a flat view for a device and its registrations.
    /// Typically used by the UI.
    /// </summary>
    public class DeviceWithRegistrationsFlatView
    {
        /// <summary>
        /// Device's ID inside the system.
        /// </summary>
        /// <example>1</example>
        public int DeviceId { get; set; }

        /// <summary>
        /// Device's serial number.
        /// </summary>
        /// <example>567s87ds32fdawe54fd54sdfsd545</example>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Device's model name.
        /// </summary>
        /// <example>SmartAC_prototype</example>
        public string ModelName { get; set; }

        /// <summary>
        /// Device's firmware version.
        /// </summary>
        /// <example>SmartAC_prototype</example>
        public string FirmwareVersion { get; set; }

        /// <summary>
        /// Registration timestamp
        /// </summary>
        /// <example>SmartAC_prototype</example>
        public DateTime RegistrationTS { get; set; }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public DeviceWithRegistrationsFlatView() { }
    }

    /// <summary>
    /// Key to retrieve all <see cref="DeviceModel"/> at once.
    /// </summary>
    public class DeviceAllKey : IModelKey<DeviceModel> { }

    /// <summary>
    /// Key to search <see cref="DeviceModel"/> by its <see cref="DeviceModel.DeviceId"/>.
    /// </summary>
    public class DeviceIdKey : IModelKey<DeviceModel>
    {
        /// <inheritdoc cref="DeviceModel.DeviceId"/>
        public int DeviceId { get; set; }
    }

    /// <summary>
    /// Key to search <see cref="DeviceModel"/> by its <see cref="DeviceModel.SerialNumber"/>.
    /// </summary>
    public class DeviceSerialNumberKey : IModelKey<DeviceModel>
    {
        /// <inheritdoc cref="DeviceModel.SerialNumber"/>
        public string SerialNumber { get; set; }
    }
}
