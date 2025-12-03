using ConsolidacaoVendas.Models;
using MongoDB.Driver;

namespace ConsolidacaoVendas.Repositories
{
    public interface IVendaConsolidadaRepository
    {
        IAsyncCursor<Venda> GetVendasCursor();
        Task<long> CountVendasAsync(CancellationToken ct);
        Task<Cliente?> GetClienteByIdAsync(string id, CancellationToken ct);
        Task<Empresa?> GetEmpresaByIdAsync(string id, CancellationToken ct);
        Task<PlanoDeConta?> GetPlanoByIdAsync(string id, CancellationToken ct);
        Task InsertVendasConsolidadasBulkAsync(IEnumerable<VendaConsolidada> items, CancellationToken ct);
    }
}
