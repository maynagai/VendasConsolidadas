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
            [FromQuery] string? empresa,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 1000)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 1000;

            var coll = _ctx.DestinoDb.GetCollection<VendaConsolidada>("VendaConsolidada");
            var builder = Builders<VendaConsolidada>.Filter;

            var filters = new List<FilterDefinition<VendaConsolidada>>();

            if (from.HasValue)
                filters.Add(builder.Gte(x => x.Data, from.Value));

            if (to.HasValue)
                filters.Add(builder.Lte(x => x.Data, to.Value));

            if (!string.IsNullOrWhiteSpace(empresa))
                filters.Add(builder.Eq(x => x.EmpresaNome, empresa));

            var finalFilter =
                filters.Count == 0 ? FilterDefinition<VendaConsolidada>.Empty
                                   : builder.And(filters);

            var total = await coll.CountDocumentsAsync(finalFilter);

            var skip = (page - 1) * pageSize;

            var items = await coll.Find(finalFilter)
                                  .Skip(skip)
                                  .Limit(pageSize)
                                  .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                total,
                totalPages = (int)Math.Ceiling((double)total / pageSize),
                items
            });
        }


        [HttpPost("Cancel")]
        public IActionResult PostCancel()
        {
            _service.Cancel();
            return Ok(new { message = "Pedido de cancelamento enviado" });
        }

        [HttpGet("Progress")]
        public IActionResult Progress() => Ok(new { 
            progresso = _service.GetProgress() +"%",
            total = _service.GetTotal(),
            processados =_service.GetProcessed()

        });

        [HttpGet("Logs")]
        public IActionResult Logs() => Ok(_service.GetLogs());


    }
}
