using IpoApp.Core.Services;
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
    }
}
