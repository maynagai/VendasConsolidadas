namespace ConsolidacaoVendas.Services
{
    public interface IVendasConsolidadasService
    {
        Task start();
        IEnumerable<string> GetLogs();

    }
}
