using System;

namespace SmartAirControl.Models.User
{
    /// <summary>
    /// Defines the possible types of user.
    /// </summary>
    public enum UserType : byte
    {
        /// <summary>
        /// Administrator users.
        /// </summary>
        Admin = 1,
        /// <summary>
        /// Non-administrator users.
        /// </summary>
        User = 2
    }

    /// <summary>
    /// Defines a model that represents a User Of the system
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// User's ID. This is the user unique identifier for the API.
        /// </summary>
        /// <example>1</example>
        public int UserId { get; set; }

        /// <summary>
        /// User's username. This is the user unique identifier for the biz.
        /// </summary>
        /// <example>admin</example>
        public string Username { get; set; }

        /// <summary>
        /// User's password.
        /// Only used to create a new user. When retrieving the user from the DB it will be masked.
        /// </summary>
        /// <example>*****</example>
        public string Password { get; set; }

        /// <summary>
        /// User's type.
        /// <see cref="UserType"/> for more info.
        /// </summary>
        /// <example>Admin</example>
        public UserType Type { get; set; }

        /// <summary>
        /// Timestamp from when the user was created
        /// </summary>
        /// <example>2021-10-02T12:47:49.134Z</example>
        public DateTime InsertTimestamp { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public UserModel() { }
    }
}
