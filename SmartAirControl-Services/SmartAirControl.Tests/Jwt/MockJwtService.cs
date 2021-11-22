using SmartAirControl.API.Core.Jwt;
using System;
using System.Collections.Generic;

namespace SmartAirControl.Tests.Jwt
{
    internal class MockJwtService : IJwtService
    {
        public TokenInfo GenerateJwtToken(IReadOnlyDictionary<string, string> payload) =>
            new TokenInfo("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWV9.dyt0CoTl4WoVjAHI9Q_CwSKhl6d_9rhM3NrXuJttkao", DateTime.UtcNow.AddSeconds(3600));
    }
}
