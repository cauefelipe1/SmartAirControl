using Dapper;
using SmartAirControl.API.Database;

namespace SmartAirControl.API.Features.User
{
    public class UserPostgreSqlRepository : IUserRepository
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
                ""user""
            WHERE
                UserName = @Username";

        public UserPostgreSqlRepository(DapperContext dapperContext) => _dapperContext = dapperContext;

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
                INSERT INTO ""user"" (
                    UserName, 
                    Password, 
                    UserType, 
                    InsertTS
                ) 
                VALUES (
                    @UserName, 
                    @Password, 
                    @UserType, 
                    @InsertTS
                )
                RETURNING UserId";

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
