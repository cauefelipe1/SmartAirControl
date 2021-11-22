using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAirControl.API.Core.Jwt;
using SmartAirControl.Models.Login;
using System.Threading.Tasks;

namespace SmartAirControl.API.Features.Authentication
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator) => _mediator = mediator;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<TokenInfo>> Login([FromBody] LoginModel loginInfo)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var token = await _mediator.Send(new AuthenticationMediator.AuthenticateUserRequest(loginInfo));

            return Ok(token);
        }
    }
}
