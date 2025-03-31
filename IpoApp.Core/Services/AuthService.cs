using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using IpoApp.Models.Entities;
using IpoApp.Repository;


namespace IpoApp.Core.Services
{
    // Services/AuthService.cs
    public interface IAuthService
    {
        Task<AuthResponse> AuthenticateAsync(LoginRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> AuthenticateAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.UserID, request.OrgShortCode);
            if (user == null)
                throw new AuthenticationException("Invalid credentials");

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new AuthenticationException("Invalid credentials");

            if (!user.IsActive)
                throw new AuthenticationException("User account is inactive");

            return _jwtService.GenerateToken(user);
        }
    }
}
