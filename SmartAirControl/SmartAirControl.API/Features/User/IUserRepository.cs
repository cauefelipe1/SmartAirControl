namespace SmartAirControl.API.Features.User
{
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a <see cref="UserDTO"/> from the database based on the username.
        /// </summary>
        /// <param name="username">User's username.</param>
        /// <returns>Instance of <see cref="UserDTO"/></returns>
        UserDTO GetUser(string username);

        /// <summary>
        /// Saves a <see cref="UserDTO"/> into the database.
        /// </summary>
        /// <param name="dto">UserDTO with the user information.</param>
        /// <returns>User id generated during the insert.</returns>
        int SaveUser(UserDTO dto);
    }
}
