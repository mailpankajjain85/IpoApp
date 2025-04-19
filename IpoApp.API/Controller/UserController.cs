using IpoApp.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IpoApp.API.Controller
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service) => _service = service;

        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create(UserCreateRequest request)
        {
            try
            {
                var user = await _service.CreateUserAsync(request);
                return CreatedAtAction(nameof(Get), new { id = user.UserID }, user);
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> Get(Guid id)
        {
            var client = await _service.GetUserAsync(id);
            return client != null ? Ok(client) : NotFound();
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
        {
            var users = await _service.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserUpdateRequest request)
        {
            try
            {
                await _service.UpdateUserAsync(userId, request);
                return NoContent();
            }
            catch (Exception ex) when (ex is KeyNotFoundException or UnauthorizedAccessException)
            {
                return Problem(
                    title: ex.GetType().Name,
                    detail: ex.Message,
                    statusCode: ex is KeyNotFoundException ? 404 : 403);
            }
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                await _service.DeleteUserAsync(userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Problem(
                    title: "Forbidden",
                    detail: ex.Message,
                    statusCode: 403);
            }
        }
    }
}
