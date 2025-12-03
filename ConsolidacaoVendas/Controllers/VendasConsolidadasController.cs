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

        [HttpGet("GetProgress")]
        public IActionResult Progress() => Ok(new { progress = _service.GetProgress() });

        [HttpGet("GetLogs")]
        public IActionResult Logs() => Ok(_service.GetLogs());


    }
}
