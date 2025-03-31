using IpoApp.Repository;
using IpoApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using IpoApp.Core.Services;
using System.Runtime.CompilerServices;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Connections;
using System.Security.Authentication;

namespace IpoApp.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.AuthenticateAsync(request);
                return Ok(response);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }


        //[HttpPost("login")]
        //public IActionResult Login(string userName, string password, string orgShortCode)
        //{
        //    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        //    {
        //        return BadRequest("Username and password are required.");
        //    }
        //    // Authenticate the user using the UserService
        //    var authenticatedUser = _userService.Authenticate(userName, password, orgShortCode);
        //    if (authenticatedUser == null)
        //    {
        //        return Unauthorized("Invalid username or password.");
        //    }

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // Replace with your actual secret key
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim(ClaimTypes.Name, authenticatedUser.Name),
        //            new Claim(ClaimTypes.Email, authenticatedUser.Email),
        //            new Claim("OrgShortCode", authenticatedUser.OrgShortCode),
        //            new Claim(ClaimTypes.MobilePhone, authenticatedUser.Phone), // Use UserId or another unique identifier
        //            new Claim("UserId", authenticatedUser.UserName),
        //            new Claim("Role", authenticatedUser.UserRole.ToString())
        //        }),
        //        Expires = DateTime.UtcNow.AddHours(1),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    var tokenString = tokenHandler.WriteToken(token);
        //    // Return the token along with user information 

        //    return Ok(tokenString);
        //}

    }
}