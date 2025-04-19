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

        [HttpGet("validate-token")]
        [Authorize] // Requires valid token
        public IActionResult ValidateToken()
        {
            // Extract claims from the validated token
            var userClaims = HttpContext.User.Claims;

            // Create response object
            var response = new
            {
                IsValid = true,
                UserDetails = new
                {
                    UserId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                    Username = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                    Email = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                    Role = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                    OrgShortCode = userClaims.FirstOrDefault(c => c.Type == "OrgShortCode")?.Value,
                    TokenIssuedAt = DateTimeOffset.FromUnixTimeSeconds(
                        long.Parse(userClaims.FirstOrDefault(c => c.Type == "iat")?.Value)).DateTime,
                    TokenExpiresAt = DateTimeOffset.FromUnixTimeSeconds(
                        long.Parse(userClaims.FirstOrDefault(c => c.Type == "exp")?.Value)).DateTime
                }
            };

            return Ok(response);
        }


    }
}