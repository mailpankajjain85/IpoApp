using IpoApp.Models.Entities;
using IpoApp.Repository;

namespace IpoApp.Core.Services
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(UserCreateRequest request);
        Task<UserResponse> GetUserAsync(Guid id);

        //User GetById(int id);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IPasswordHasher _hasher;

        public UserService(IUserRepository userRepo, IClientRepository clientRepo, IPasswordHasher hasher)
        {
            _userRepo = userRepo;
            _clientRepo = clientRepo;
            _hasher = hasher;
        }

        public async Task<UserResponse> CreateUserAsync(UserCreateRequest request)
        {
            if (await _userRepo.UsernameExistsAsync(request.Username))
                throw new InvalidOperationException("Username already exists");

            if (request.Role == "CLIENT" && !request.ClientID.HasValue)
                throw new ArgumentException("ClientID is required for CLIENT role");

            // Manual mapping
            var user = new User
            {
                UserID = Guid.NewGuid(),
                OrgShortCode = request.OrgShortCode,
                Username = request.Username,
                PasswordHash = _hasher.HashPassword(request.Password),
                Email = request.Email,
                Role = request.Role,
                FullName = request.FullName,
                ClientID = request.ClientID,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepo.CreateAsync(user);

            // Manual mapping to response
            var response = new UserResponse
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                ClientID = user.ClientID
            };

            // Add client short code if available
            if (user.ClientID.HasValue)
            {
                var client = await _clientRepo.GetByIdAsync(user.ClientID.Value);
                response.ClientShortCode = client?.ClientShortCode;
            }

            return response;
        }
        public async Task<UserResponse> GetUserAsync(Guid id)
        {
            var client = await _clientRepo.GetByIdAsync(id);
            if (client == null) return null;

            return new UserResponse()
            {
                ClientID = client.ClientID,
                ClientShortCode = client.ClientShortCode,
                Email = client.Email,
                FullName = client.FullName,
                IsActive = client.IsActive
            };
        }
    }
}
    // Your code here
