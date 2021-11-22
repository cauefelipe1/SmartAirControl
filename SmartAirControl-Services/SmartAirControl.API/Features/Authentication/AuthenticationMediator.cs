using MediatR;
using SmartAirControl.API.Core.Jwt;
using SmartAirControl.API.Features.User;
using SmartAirControl.Models.Authentication;
using SmartAirControl.Models.Device;
using SmartAirControl.Models.Login;
using SmartAirControl.Models.User;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAirControl.API.Features.Authentication
{
    public class AuthenticationMediator
    {
        private static TokenInfo GetUserTokenInfo(UserModel user, IJwtService jwtService, DeviceRegistrationModel registration = null)
        {
            var claims = new Dictionary<string, string>
            {
                [nameof(IdentityClaimsModel.UserId)] = user.UserId.ToString(),
                [nameof(IdentityClaimsModel.Username)] = user.Username
            };

            if (registration is not null)
            {
                claims[nameof(IdentityClaimsModel.DeviceRegistrationId)] = registration.DeviceRegistrationId.ToString();
                claims[nameof(IdentityClaimsModel.DeviceFirmwareVersion)] = registration.FirmwareVersion;

            }

            var token = jwtService.GenerateJwtToken(claims);

            return token;
        }

        #region Authenticate_User
        public class AuthenticateUserRequest : IRequest<TokenInfo>
        {
            public LoginModel Model { get; }

            public AuthenticateUserRequest(LoginModel model)
            {
                Model = model;
            }
        }

        public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserRequest, TokenInfo>
        {
            private readonly IMediator _mediator;
            private readonly IJwtService _jwtService;

            public AuthenticateUserHandler(IMediator mediator, IJwtService jwtService)
            {
                _mediator = mediator;
                _jwtService = jwtService;
            }

            public async Task<TokenInfo> Handle(AuthenticateUserRequest request, CancellationToken cancellationToken)
            {
                string username = request.Model.Username;

                var user = await _mediator.Send(new UserMediator.GetUserRequest(username));

                if (user is not null)
                {
                    var token = GetUserTokenInfo(user, _jwtService);

                    return token;
                }
                else
                {
                    throw new Exception("Invalid username and/or password.");
                }
            }
        }
        #endregion Authenticate_User
    }
}
