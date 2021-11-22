using Microsoft.AspNetCore.Mvc;
using SmartAirControl.Models.Authentication;
using System;

namespace SmartAirControl.API.Core.Extensions
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Returns the claims about the authenticated user who made the Http request.
        /// </summary>
        public static IdentityClaimsModel GetIdentityClaims(this ControllerBase controller)
        {
            var result = new IdentityClaimsModel();

            if (controller.HttpContext.Items.TryGetValue(nameof(IdentityClaimsModel.UserId), out object userId))
                result.UserId = Convert.ToInt32(userId);

            if (controller.HttpContext.Items.TryGetValue(nameof(IdentityClaimsModel.Username), out object username))
                result.DeviceFirmwareVersion = username.ToString();

            if (controller.HttpContext.Items.TryGetValue(nameof(IdentityClaimsModel.DeviceRegistrationId), out object registrationId))
                result.DeviceRegistrationId = Convert.ToInt32(registrationId);

            if (controller.HttpContext.Items.TryGetValue(nameof(IdentityClaimsModel.DeviceFirmwareVersion), out object firmware))
                result.DeviceFirmwareVersion = firmware.ToString();

            return result;
        }
    }
}
