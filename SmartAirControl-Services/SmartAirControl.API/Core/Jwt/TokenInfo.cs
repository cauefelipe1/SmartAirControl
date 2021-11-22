using System;

namespace SmartAirControl.API.Core.Jwt
{
    /// <summary>
    /// Defines a token information
    /// </summary>
    public sealed class TokenInfo
    {
        /// <summary>
        /// String that describes the type of the token.
        /// </summary>
        /// <example>Bearer</example>
        public string TokenType { get; }

        /// <summary>
        /// Encoded token.
        /// </summary>
        /// <example>px9qge8BMw9pgUH2fkC9kY5b84zeJf3f.pfNjhsvEMxdkKhjS3rXuyLBAXYYrej4Y.uLjrJX9xXL3xT6NpdtrxTyp5qeV9K8bG</example>
        public string Token { get; }

        /// <summary>
        /// Date and time of token expiration.
        /// </summary>
        /// <example>2021-10-02T12:47:49.134Z</example>
        public DateTime ExpiresIn { get; }

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="tokenType"><see cref="TokenType"/></param>
        /// <param name="token"><see cref="Token"/></param>
        /// <param name="expiresIn"><see cref="ExpiresIn"/></param>
        public TokenInfo(string tokenType, string token, DateTime expiresIn)
        {
            TokenType = tokenType;
            Token = token;
            ExpiresIn = expiresIn;
        }
    }
}
