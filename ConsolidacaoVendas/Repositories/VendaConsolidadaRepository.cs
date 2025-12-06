using ConsolidacaoVendas.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using ConsolidacaoVendas.Mongo;

namespace ConsolidacaoVendas.Repositories
{
    public class VendaConsolidadaRepository : IVendaConsolidadaRepository
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

            CriarIndicesSeNecessario();
        }

        private void CriarIndicesSeNecessario()
        {
            // Lookup via ExternalId ficou super rápido
            _clientes.Indexes.CreateOne(
                new CreateIndexModel<Cliente>(
                    Builders<Cliente>.IndexKeys.Ascending(c => c.ExternalId),
                    new CreateIndexOptions { Unique = false }));

            _empresas.Indexes.CreateOne(
                new CreateIndexModel<Empresa>(
                    Builders<Empresa>.IndexKeys.Ascending(e => e.ExternalId),
                    new CreateIndexOptions { Unique = false }));

            _planos.Indexes.CreateOne(
                new CreateIndexModel<PlanoDeConta>(
                    Builders<PlanoDeConta>.IndexKeys.Ascending(p => p.ExternalId),
                    new CreateIndexOptions { Unique = false }));
        }

        public IAsyncCursor<Venda> GetVendasCursor()
        {
            return _vendas.FindSync(
                filter: Builders<Venda>.Filter.Empty,
                options: new FindOptions<Venda>
                {
                    BatchSize = 2000,
                    NoCursorTimeout = false
                });
        }

        public Task<long> CountVendasAsync(CancellationToken ct)
        {
            return _vendas.CountDocumentsAsync(
                filter: Builders<Venda>.Filter.Empty,
                cancellationToken: ct
            );
        }
        public IMongoCollection<VendaConsolidada> GetVendasConsolidadasCollection()
        {
            return _ctx.DestinoDb.GetCollection<VendaConsolidada>("VendaConsolidada");
        }

        public async Task<List<Cliente>> GetAllClientesAsync(CancellationToken ct)
        {
            return await _clientes.Find(FilterDefinition<Cliente>.Empty)
                                  .ToListAsync(ct);
        }

        public async Task<List<Empresa>> GetAllEmpresasAsync(CancellationToken ct)
        {
            return await _empresas.Find(FilterDefinition<Empresa>.Empty)
                                  .ToListAsync(ct);
        }

        public async Task<List<PlanoDeConta>> GetAllPlanosAsync(CancellationToken ct)
        {
            return await _planos.Find(FilterDefinition<PlanoDeConta>.Empty)
                                .ToListAsync(ct);
        }


        public Task InsertVendasConsolidadasBulkAsync(
            IEnumerable<VendaConsolidada> items,
            CancellationToken ct)
        {

            var options = new InsertManyOptions
            {
                IsOrdered = false, // Não trava em erro
                BypassDocumentValidation = true // + desempenho
            };

            return _vendasConsolidadas.InsertManyAsync(items, options, ct);
        }
    }
}


