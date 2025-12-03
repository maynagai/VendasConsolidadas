using ConsolidacaoVendas.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using ConsolidacaoVendas.Mongo;

namespace ConsolidacaoVendas.Repositories
{
    public class VendaConsolidadaRepository
    {
        private readonly MongoContext _ctx;
        private readonly IMongoCollection<Venda> _vendas;
        private readonly IMongoCollection<Cliente> _clientes;
        private readonly IMongoCollection<Empresa> _empresas;
        private readonly IMongoCollection<PlanoDeConta> _planos;
        private readonly IMongoCollection<VendaConsolidada> _vendasConsolidadas;

        public VendaConsolidadaRepository(MongoContext ctx)
        {
            _ctx = ctx;
            _vendas = _ctx.OrigemDb.GetCollection<Venda>("Venda");
            _clientes = _ctx.OrigemDb.GetCollection<Cliente>("Cliente");
            _empresas = _ctx.OrigemDb.GetCollection<Empresa>("Empresa");
            _planos = _ctx.OrigemDb.GetCollection<PlanoDeConta>("PlanoDeConta");
            _vendasConsolidadas = _ctx.DestinoDb.GetCollection<VendaConsolidada>("VendaConsolidada");
        }

        public IAsyncCursor<Venda> GetVendasCursor()
        {
            var filter = Builders<Venda>.Filter.Empty;
            var options = new FindOptions<Venda> { BatchSize = 1000 };
            return _vendas.FindSync(filter, options);
        }

        public Task<Cliente?> GetClienteByIdAsync(string id, CancellationToken ct) =>
            _clientes.Find(c => c.Id == id).FirstOrDefaultAsync(ct);

        public Task<Empresa?> GetEmpresaByIdAsync(string id, CancellationToken ct) =>
            _empresas.Find(e => e.Id == id).FirstOrDefaultAsync(ct);

        public Task<PlanoDeConta?> GetPlanoByIdAsync(string id, CancellationToken ct) =>
            _planos.Find(p => p.Id == id).FirstOrDefaultAsync(ct);

        public Task InsertVendasConsolidadasBulkAsync(IEnumerable<VendaConsolidada> items, CancellationToken ct) =>
            _vendasConsolidadas.InsertManyAsync(items, cancellationToken: ct);
    }
}

