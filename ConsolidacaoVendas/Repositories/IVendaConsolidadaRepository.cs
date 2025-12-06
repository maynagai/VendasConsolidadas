using ConsolidacaoVendas.Models;
using MongoDB.Driver;

namespace ConsolidacaoVendas.Repositories
{
    public interface IVendaConsolidadaRepository
    {
        IAsyncCursor<Venda> GetVendasCursor();

        Task<long> CountVendasAsync(CancellationToken ct);

        Task<List<Cliente>> GetAllClientesAsync(CancellationToken ct);
        Task<List<Empresa>> GetAllEmpresasAsync(CancellationToken ct);
        Task<List<PlanoDeConta>> GetAllPlanosAsync(CancellationToken ct);

        IMongoCollection<VendaConsolidada> GetVendasConsolidadasCollection();
        Task InsertVendasConsolidadasBulkAsync(IEnumerable<VendaConsolidada> items, CancellationToken ct);
    }

}
