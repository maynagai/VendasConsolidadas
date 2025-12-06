namespace ConsolidacaoVendas.Services
{
    public interface IVendasConsolidadasService
    {
        Task Start();
        void Cancel();
        IEnumerable<string> GetLogs();
        int GetProgress();
        long GetTotal();
        long GetProcessed();

    }
}
