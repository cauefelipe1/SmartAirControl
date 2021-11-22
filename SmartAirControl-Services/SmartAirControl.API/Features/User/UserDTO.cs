using System;

namespace SmartAirControl.API.Features.User
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int UserType { get; set; }

        public DateTime InsertTS { get; set; }
    }
}
