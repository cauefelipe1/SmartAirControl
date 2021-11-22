namespace SmartAirControl.Models.Login
{
    /// <summary>
    /// Defines a user login model to be used during the login process.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Username for a user.
        /// </summary>
        /// <example>admin</example>
        public string Username { get; set; }

        /// <summary>
        /// User's password.
        /// </summary>
        /// <example>123456</example>
        public string Password { get; set; }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public LoginModel() { }
    }
}
