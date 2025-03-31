using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IpoApp.Core.Services
{
    // Services/ICurrentContext.cs
    public interface ICurrentContext
    {
        string OrgShortCode { get; }
        string Role { get; }
        string Username { get; }
    }

    // Services/CurrentContext.cs
    public class CurrentContext : ICurrentContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string OrgShortCode =>
            _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(c => c.Type == "OrgShortCode")?.Value;

        public string Role =>
            _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        public string Username =>
            _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

  
    }
}
