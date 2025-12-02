using Microsoft.AspNetCore.Mvc;
using ConsolidacaoVendas.Services;

namespace ConsolidacaoVendas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendasConsolidadasController : ControllerBase
    {
        private readonly ILogger<VendasConsolidadasController> _logger;
        private readonly IVendasConsolidadasService _service;
        public VendasConsolidadasController(ILogger<VendasConsolidadasController> logger)
        {
            _logger = logger;
        }

        [HttpPost("PostStart")]
        public IActionResult PostStart()
        {
            try {
                _=_service.start();
                return Accepted(new { message = "Processo de consolidação iniciado" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("GetProgress")]
        public IActionResult GetProgress()
        {
            return Ok();
        }
        [HttpGet("GetLogs")]
        public IActionResult GetLogs()
        {
            return Ok();
        }
        [HttpGet("GetVendasConsolidadas")]
        public IActionResult GetVendasConsolidadas([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string? empresa)
        {
            return Ok();
        }


        [HttpPost("PostCancel")]
        public IActionResult PostCancel()
        {
            return Ok();
        }



    }
}
