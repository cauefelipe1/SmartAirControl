using SmartAirControl.API.Features.Device;
using SmartAirControl.Models.Device;
using System;
using System.Collections.Generic;

namespace SmartAirControl.Tests.Device
{
    internal class DeviceFixture
    {
        private static DateTime _registrationDate = DateTime.UtcNow.AddMinutes(-10);

        internal static List<DeviceRegistrationDTO> Registrations { get; } = new()
        {
            new()
            {
                RegistrationId = 1,
                DeviceId = 1,
                FirmwareVersion = "Mark_I_V1",
                RegistrationTS = _registrationDate
            },
            new()
            {
                RegistrationId = 2,
                DeviceId = 2,
                FirmwareVersion = "Mark_I_V1",
                RegistrationTS = _registrationDate
            }
        };

        internal static List<DeviceDTO> Devices { get; } = new()
        {
            new()
            {
                DeviceId = 1,
                DeviceModel = "Mark_I",
                SerialNumber = "btJA5Px8LSaG2yCA",
                DeviceSecret = "^>L6H3a.{pcFX>{dCp]8c~-t;~Y#s-EgR49L`P2WZ%_PW`7#U'j3@5#cbfe\r)3?'Gj`d{v#Fu3",
                InsertTS = _registrationDate
            },
            new()
            {
                DeviceId = 2,
                DeviceModel = "Mark_I",
                SerialNumber = "yaF4PuHwGDUDyHWM",
                DeviceSecret = ";w(vCe,:{EhP7/$tTCQ%V8<Nw4T9;L^sBvw/Q~fv9!7B}2#&n*yyY{FyS7t=dx?;Hu,g*G~<j]G",
                InsertTS = _registrationDate
            }
        };
    }
}
