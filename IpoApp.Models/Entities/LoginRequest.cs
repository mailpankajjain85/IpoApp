using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpoApp.Models.Entities
{
    // DTOs/Auth/LoginRequest.cs
    public class LoginRequest
    {
        [Required]
        public string UserID { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        public string OrgShortCode { get; set; }
    }

    // DTOs/Auth/AuthResponse.cs
    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
        public string OrgShortCode { get; set; }
    }
}
