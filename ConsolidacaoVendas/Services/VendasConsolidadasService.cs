using static ConsolidacaoVendas.Services.VendasConsolidadasService;
using  ConsolidacaoVendas.Log;

namespace ConsolidacaoVendas.Services
{
        public class VendasConsolidadasService : IVendasConsolidadasService
        {

        private readonly LogTracker log;
            public async Task start()
            {

            }

        public IEnumerable<string> GetLogs() => log.GetAll();
    }
        
}
