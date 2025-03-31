using IpoApp.Repository;
using IpoApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using IpoApp.Core.Services;

namespace IpoApp.API.Controllers
{

    // Controllers/IposController.cs
    [ApiController]
    [Route("api/ipos")]
    public class IposController : ControllerBase
    {
        private readonly IIpoService _ipoService;

        public IposController(IIpoService ipoService)
        {
            _ipoService = ipoService;
        }

        [HttpPost]
        public async Task<ActionResult<IpoResponse>> CreateIpo(IpoCreateRequest request)
        {
            try
            {
                var ipo = await _ipoService.CreateIpoAsync(request);
                return CreatedAtAction(nameof(GetIpo), new { id = ipo.ID }, ipo);
            }
            catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IpoResponse>> GetIpo(Guid id)
        {
            var ipo = await _ipoService.GetIpoAsync(id);
            return ipo != null ? Ok(ipo) : NotFound();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IpoResponse>>> GetByOrg()
        {
            var ipos = await _ipoService.GetIposByOrgAsync();
            return Ok(ipos);
        }
    }
}
