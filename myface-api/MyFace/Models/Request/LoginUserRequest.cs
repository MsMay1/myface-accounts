using System.ComponentModel.DataAnnotations;

namespace MyFace.Models.Request
{
    public class LoginUserRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}