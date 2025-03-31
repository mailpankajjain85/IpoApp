using IpoApp.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace IpoApp.API.Controller
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _service;

        public ClientsController(IClientService service) => _service = service;

        [HttpPost]
        public async Task<ActionResult<ClientResponse>> Create(ClientCreateRequest request)
        {
            try
            {
                var client = await _service.CreateClientAsync(request);
                return CreatedAtAction(nameof(Get), new { id = client.ClientID }, client);
            }
            catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientResponse>> Get(Guid id)
        {
            var client = await _service.GetClientAsync(id);
            return client != null ? Ok(client) : NotFound();
        }
    }
}
