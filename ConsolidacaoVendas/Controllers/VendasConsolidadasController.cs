using ConsolidacaoVendas.Models;
using ConsolidacaoVendas.Mongo;
using ConsolidacaoVendas.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ConsolidacaoVendas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendasConsolidadasController : ControllerBase
    {
        private readonly IVendasConsolidadasService _service;
        private readonly MongoContext _ctx;
        public VendasConsolidadasController(IVendasConsolidadasService service, MongoContext ctx)
        {
            _service = service;
            _ctx = ctx;
        }

        [HttpPost("Start")]
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


        [HttpGet("VendasConsolidadas")]
        public async Task<IActionResult> GetVendasConsolidadasDetalhadasAsync(
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to,
    [FromQuery] string? empresa)
        {
            var coll = _ctx.DestinoDb.GetCollection<VendaConsolidada>("VendaConsolidada");
            var builder = Builders<VendaConsolidada>.Filter;
            var filter = builder.Empty;

            if (from.HasValue) filter &= builder.Gte(x => x.Data, from.Value);
            if (to.HasValue) filter &= builder.Lte(x => x.Data, to.Value);
            if (!string.IsNullOrEmpty(empresa)) filter &= builder.Eq(x => x.EmpresaNome, empresa);


            var list = await coll.Find(filter).ToListAsync();

            return Ok(list);
        }


        [HttpPost("Cancel")]
        public IActionResult PostCancel()
        {
            _service.Cancel();
            return Ok(new { message = "Pedido de cancelamento enviado" });
        }

        [HttpGet("Progress")]
        public IActionResult Progress() => Ok(new { progress = _service.GetProgress() });

        [HttpGet("Logs")]
        public IActionResult Logs() => Ok(_service.GetLogs());


    }
}
