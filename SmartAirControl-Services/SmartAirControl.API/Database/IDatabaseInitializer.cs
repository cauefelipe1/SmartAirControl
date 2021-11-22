using SmartAirControl.API.Core.Settings;
using System;
using System.Collections.Generic;

namespace SmartAirControl.API.Database
{
    public interface IDatabaseInitializer
    {
        void InitializeDatabase(bool forceCreate = false);
    }

    internal static class DataExampleProvider
    {
        public static IEnumerable<object> GetUsersDataExample()
        {
            return new []
            {
                new {
                    UserId = 1,
                    UserName = "admin",
                    Password = "123456",
                    UserType = 1,
                    InsertTS = DateTime.UtcNow
                },
                new {
                    UserId = 2,
                    UserName = "user",
                    Password = "123456",
                    UserType = 2,
                    InsertTS = DateTime.UtcNow
                }
            };
        }

        public static IEnumerable<object> GetDevicesDataExample()
        {
            return new[]
            {
                new {
                    DeviceId = 1,
                    SerialNumber = "H75QfssMqqwHQkNCYpbn96S4LbubWx",
                    DeviceSecret = "jQL5ddEQGu8RuwVYWX5c8vm4W",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 2,
                    SerialNumber = "BVTr7eu5JTLtthZSkqFLz3pRvXhEKc",
                    DeviceSecret = "8X4jrnkgURdbvmCCtx9z6nyTS",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 3,
                    SerialNumber = "M7wGP39qnkBKzhWyRF8BXgW5zvrLfE7z",
                    DeviceSecret = "fTsyqumzQMLgSNxZ4UUbh87e46L",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 4,
                    SerialNumber = "xESYN2xKHshAreFdG9CGbFkvqbtA7R4a",
                    DeviceSecret = "N2xKHshAreFdG9CGbF",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 5,
                    SerialNumber = "nen6Be83CzJu2DWjPYUvnRjqY4tpevvk",
                    DeviceSecret = "nen6Be83CzJu2DWjPvvk",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 6,
                    SerialNumber = "bnDuLtgs2cnxda5JT6SHG6khpPfJsPg4",
                    DeviceSecret = "bnDuLts2caSpPfJs",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 7,
                    SerialNumber = "6qw2HkM2em32eZxENSH3c7TBzGnQGeqA",
                    DeviceSecret = "m32eZxENSH3c7TBzGn",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 8,
                    SerialNumber = "GQvwWHeaBsQSdu5BmxvhhQfv3wGgFU45",
                    DeviceSecret = "wWHsQSdu5BmxvhhQfv3wGgFU",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 9,
                    SerialNumber = "VF73AcZpCcQ5Pw64zt5mSXaXpMyRdf6M",
                    DeviceSecret = "srewrefhftg45",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                },
                new {
                    DeviceId = 10,
                    SerialNumber = "QSFRg674UEb3rBNmNmujSBPtDjc85FkZ",
                    DeviceSecret = "rBNmNmujSBPtDjc",
                    DeviceModel = "SmartAC_prototype",
                    InsertTS = DateTime.UtcNow
                }
            };
        }
    }
}
