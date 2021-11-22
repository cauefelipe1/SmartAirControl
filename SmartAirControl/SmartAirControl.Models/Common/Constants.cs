using SmartAirControl.Models.Device;
using System.Collections.Generic;

namespace SmartAirControl.Models.Common
{
    public readonly struct DeviceSensorRange
    {
        public double MinValue { get; }

        public double MaxValue { get; }

        public DeviceSensorRange(double minValue, double maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }

    public static class Constants
    {
        public static class Device
        {
            public class Health
            {
                public const string OK = "OK";
                public const string NEEDS_FILTER = "needs_filter";
                public const string NEEDS_SERVICE = "needs_service";

                public static readonly string[] HEALTH_STATUS_VALUES = new[] { OK, NEEDS_FILTER, NEEDS_SERVICE };
            }

            public static class Sensors
            {
                public static readonly IReadOnlyDictionary<SensorType, DeviceSensorRange> SENSOR_RANGES =
                new Dictionary<SensorType, DeviceSensorRange>
                {
                    {SensorType.Temperature, new (-30.00, 100.00)},
                    {SensorType.Humidity, new (0, 100.00)},
                    {SensorType.CarbonMonoxide, new (0, 1_000.00)}
                };

                public const double CARBON_THRESHOLD = 9.0;
            }

            public const int UNREADABLE_REPORTS_LIMIT = 500;
        }
    }
}
