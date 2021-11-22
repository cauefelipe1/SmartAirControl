using Dapper;
using SmartAirControl.API.Database;

namespace SmartAirControl.API.Features.User
{
    public class UserSqlServerRepository : IUserRepository
    {
        private readonly DapperContext _dapperContext;

        const string SELECT_USER = @"
            SELECT
                UserId,
                UserName, 
                Password, 
                UserType, 
                InsertTS
            FROM
                [User]
            WHERE
                UserName = @Username";

        public UserSqlServerRepository(DapperContext dapperContext) => _dapperContext = dapperContext;

        /// <inheritdoc cref="IUserRepository.GetUser(string)"/>
        public UserDTO GetUser(string username)
        {
            using (var conn = _dapperContext.CreateConnection())
            {
                var dto = conn.QueryFirstOrDefault<UserDTO>(SELECT_USER, new { username });

                return dto;
            }
        }

        const string INSERT_USER = @"
                INSERT INTO [User] (
                    UserName, 
                    Password, 
                    UserType, 
                    InsertTS
                ) 
                OUTPUT INSERTED.UserId
                VALUES (
                    @UserName, 
                    @Password, 
                    @UserType, 
                    @InsertTS
                )";

        /// <inheritdoc cref="IUserRepository.SaveUser(UserDTO)"/>
        public int SaveUser(UserDTO dto)
        {
            using (var conn = _dapperContext.CreateConnection())
            {
                int userId = conn.QuerySingle<int>(INSERT_USER, dto);

                return userId;
            }
        }
    }
}
