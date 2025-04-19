using IpoApp.Models.Entities;
using IpoApp.Repository;
using Microsoft.AspNet.Identity;
namespace IpoApp.Core.Services
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(UserCreateRequest request);
        Task<UserResponse> GetUserAsync(Guid id);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task DeleteUserAsync(Guid id);
        Task UpdateUserAsync(Guid userId, UserUpdateRequest request);
        //User GetById(int id);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IPasswordHasher _hasher;

        private readonly ICurrentContext _context;

        public UserService(IUserRepository userRepo, IClientRepository clientRepo, IPasswordHasher hasher, ICurrentContext context)
        {
            _userRepo = userRepo;
            _clientRepo = clientRepo;
            _hasher = hasher;
            _context = context;
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
            var user = await _userRepo.GetByIdAsync(id, _context.OrgShortCode);
            if (user == null) return null;

            return new UserResponse()
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                ClientID = user.ClientID
            };
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            // Only allow admins to list users
            if (!_context.IsInRole("ADMIN"))
                throw new UnauthorizedAccessException("Insufficient permissions");
            var users = await _userRepo.GetByOrgAsync(_context.OrgShortCode);
            return users.Select(user => new UserResponse
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                ClientID = user.ClientID
            });
        }

        // Delete user (soft delete)
        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId, _context.OrgShortCode);

            // Validate
            if (user == null) return;

            if (user.OrgShortCode != _context.OrgShortCode)
                throw new UnauthorizedAccessException("Cannot delete users from other organizations");

            // Only admins can delete
            if (!_context.IsInRole("ADMIN"))
                throw new UnauthorizedAccessException("Insufficient permissions");

            // Soft delete
            user.IsActive = false;
            await _userRepo.UpdateAsync(user);
        }

        // Update user
        public async Task UpdateUserAsync(Guid userId, UserUpdateRequest request)
        {
            var existingUser = await _userRepo.GetByIdAsync(userId, _context.OrgShortCode);

            // Validate
            if (existingUser == null)
                throw new KeyNotFoundException("User not found");

            if (existingUser.OrgShortCode != _context.OrgShortCode)
                throw new UnauthorizedAccessException("Cannot modify users from other organizations");

            // Authorization: Only admins or self can update
            if (!_context.IsInRole("ADMIN") && _context.Username != existingUser.Username)
                throw new UnauthorizedAccessException("Can only update your own profile");

            // Apply updates
            existingUser.Email = request.Email;
            existingUser.IsActive = request.IsActive;

            // Only admins can change roles
            if (_context.IsInRole("ADMIN") && request.Role != null)
            {
                existingUser.Role = request.Role;
            }

            await _userRepo.UpdateAsync(existingUser);
        }
    }
}
// Your code here