using IpoApp.Core.Services;
using IpoApp.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace IpoApp.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantOrgsController : ControllerBase
    {
        private readonly ITenantOrgService _service;

        public TenantOrgsController(ITenantOrgService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TenantOrg>> GetById(Guid id)
        {
            var org = await _service.GetTenantOrgByIdAsync(id);
            return org != null ? Ok(org) : NotFound();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TenantOrg>>> GetAll()
        {
            return Ok(await _service.GetAllTenantOrgsAsync());
        }

        [HttpPost]
        public async Task Create([FromBody] TenantOrgDto tenantOrg)
        {
            try
            {
                await _service.CreateTenantOrgAsync(tenantOrg);
                Task.CompletedTask.Wait();
            }
            catch (InvalidOperationException ex)
            {
                Task.FromException(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TenantOrgDto tenantOrg)
        {

            try
            {
                await _service.UpdateTenantOrgAsync(id, tenantOrg);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteTenantOrgAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
