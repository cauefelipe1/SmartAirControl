using System.Collections.Generic;

namespace SmartAirControl.API.Core.Jwt
{
    public interface IJwtService
    {
        /// <summary>
        /// Generates a <see cref="TokenInfo"/> instance containig whatever paylod passed as parameter.
        /// </summary>
        /// <param name="payload">Payload to be added into the JWT token.</param>
        /// <returns>A <see cref="TokenInfo"/> instance</returns>
        TokenInfo GenerateJwtToken(IReadOnlyDictionary<string, string> payload);
    }
}
