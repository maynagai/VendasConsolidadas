using  ConsolidacaoVendas.Log;
using ConsolidacaoVendas.Models;
using ConsolidacaoVendas.Repositories;
using MongoDB.Bson;

namespace ConsolidacaoVendas.Services
{
        public class VendasConsolidadasService : IVendasConsolidadasService
        {

        private readonly LogTracker log;
        private readonly ProgressTracker progress;
        private readonly ILogger logger;
        private readonly IVendaConsolidadaRepository repository;

        private CancellationTokenSource? cts;
        private readonly object check = new();
        private bool running = false;
        public VendasConsolidadasService(IVendaConsolidadaRepository repo, ProgressTracker prog, LogTracker logs, ILogger<VendasConsolidadasService> logg)
        {
            repository = repo;
            progress = prog;
            log = logs;
            logger = logg;
        }
        public async Task Start(){

            lock (check)
            {
                if (running) throw new InvalidOperationException("Process already running");
                running = true;
                cts = new CancellationTokenSource();
            }
            var ct = cts.Token;
            log.Add("Consolidação iniciada.");
            logger.LogInformation("Consolidação iniciada");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                progress.SetTotal(await repository.CountVendasAsync(ct));
                using var cursor = repository.GetVendasCursor();

                var batch = new List<VendaConsolidada>();
                const int BATCH_SIZE = 1000;

                while (cursor.MoveNext(ct))
                {
                    foreach (var venda in cursor.Current)
                    {
                        ct.ThrowIfCancellationRequested();

                        var empresaTask = repository.GetEmpresaByIdAsync(venda.EmpresaId, ct);
                        var planoTask = repository.GetPlanoByIdAsync(venda.PlanoDeContaId, ct);

                        await Task.WhenAll(empresaTask, planoTask);

                        var empresa = empresaTask.Result;
                        var plano = planoTask.Result;

                        Cliente? cliente = null;
                        if (empresa != null && !string.IsNullOrEmpty(empresa.ClienteId))
                        {
                            cliente = await repository.GetClienteByIdAsync(empresa.ClienteId, ct);
                        }

                        var consolidada = new VendaConsolidada
                        {
                            Id = ObjectId.GenerateNewId().ToString(),
                            IdDaVenda = venda.Id,
                            Valor = venda.Valor,
                            Data = venda.Data,
                            EmpresaNome = empresa?.Nome,
                            CnpjDaEmpresa = empresa?.CNPJ,
                            ClienteNome = cliente?.Nome,
                            NomeDoPlanoDeContas = plano?.Nome
                        };

                        batch.Add(consolidada);
                        progress.IncrementProcessed();

                        if (batch.Count >= BATCH_SIZE)
                        {
                            await repository.InsertVendasConsolidadasBulkAsync(batch, ct);
                            log.Add($"Batch inserido: {batch.Count} items.");
                            batch.Clear();
                        }
                    }
                }

                if (batch.Any())
                {
                    await repository.InsertVendasConsolidadasBulkAsync(batch, ct);
                    log.Add($"Batch final inserido: {batch.Count} items.");
                }

                watch.Stop();
                log.Add($"Consolidação concluída em {watch.Elapsed}.");
                logger.LogInformation("Consolidação concluída em {time}", watch.Elapsed);
            }
            catch (OperationCanceledException)
            {
                watch.Stop();
                log.Add("Consolidação cancelada pelo usuário.");
                logger.LogInformation("Consolidação cancelada.");
            }
            catch (Exception ex)
            {
                watch.Stop();
                log.Add($"Erro: {ex.Message}");
                logger.LogError(ex, "Erro durante consolidação");
            }
            finally
            {
                lock (check)
                {
                    running = false;
                    cts?.Dispose();
                    cts = null;
                }
            }
        }

        
        public void Cancel()
        {
            lock (check)
            {
                if (running && cts != null && !cts.IsCancellationRequested)
                {
                    cts.Cancel();
                    log.Add("Pedido de cancelamento enviado.");
                }
            }
        }


        public IEnumerable<string> GetLogs() => log.GetAll();
        public int GetProgress() => progress.PercentComplete;
    }
        
}
