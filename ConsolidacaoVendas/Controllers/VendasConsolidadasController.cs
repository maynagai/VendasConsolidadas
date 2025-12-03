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
        public VendasConsolidadasController(ILogger<VendasConsolidadasController> logger, IVendasConsolidadasService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("PostStart")]
        public IActionResult PostStart()
        {
            try {
                _service.Start();
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
            _service.Cancel();
            return Ok(new { message = "Pedido de cancelamento enviado" });
        }

        [HttpGet("GetProgress")]
        public IActionResult Progress() => Ok(new { progress = _service.GetProgress() });

        [HttpGet("GetLogs")]
        public IActionResult Logs() => Ok(_service.GetLogs());


    }
}
