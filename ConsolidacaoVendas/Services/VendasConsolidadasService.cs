using  ConsolidacaoVendas.Log;
using ConsolidacaoVendas.Models;
using ConsolidacaoVendas.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ConsolidacaoVendas.Services
{
        public class VendasConsolidadasService : IVendasConsolidadasService
        {

        private readonly LogTracker log;
        private readonly ProgressTracker progress;
        private readonly IVendaConsolidadaRepository repository;
        private readonly List<string> _consolidadasInseridas = new();

        private CancellationTokenSource? cts;
        private readonly object check = new();
        private bool running = false;
        public VendasConsolidadasService(IVendaConsolidadaRepository repo, ProgressTracker prog, LogTracker logs)
        {
            repository = repo;
            progress = prog;
            log = logs;
        }
        public async Task Start()
        {
            lock (check)
            {
                if (running) throw new InvalidOperationException("Process already running");
                running = true;
                cts = new CancellationTokenSource();
            }

            var ct = cts.Token;
            log.Add("Consolidação iniciada.");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                progress.SetTotal(await repository.CountVendasAsync(ct));

                log.Add("Carregando cache em memória...");
                var empresas = (await repository.GetAllEmpresasAsync(ct))
                    .ToDictionary(e => e.ExternalId);

                var planos = (await repository.GetAllPlanosAsync(ct))
                    .ToDictionary(p => p.ExternalId);

                var clientes = (await repository.GetAllClientesAsync(ct))
                    .ToDictionary(c => c.ExternalId);

                log.Add($"Carregado: {empresas.Count} empresas, {planos.Count} planos, {clientes.Count} clientes");

                using var cursor = repository.GetVendasCursor();

                const int BATCH_SIZE = 1000;
                var batch = new List<VendaConsolidada>(BATCH_SIZE);

                while (cursor.MoveNext(ct))
                {
                    foreach (var venda in cursor.Current)
                    {
                        ct.ThrowIfCancellationRequested();

                        empresas.TryGetValue(venda.EmpresaId, out var empresa);
                        planos.TryGetValue(venda.PlanoDeContaId, out var plano);

                        Cliente? cliente = null;
                        if (empresa != null && empresa.ClienteId != null)
                            clientes.TryGetValue(empresa.ClienteId, out cliente);

                        batch.Add(new VendaConsolidada
                        {
                            Id = ObjectId.GenerateNewId().ToString(),
                            IdDaVenda = venda.Id,
                            Valor = venda.Valor,
                            Data = venda.Data,
                            EmpresaNome = empresa?.Nome,
                            CnpjDaEmpresa = empresa?.CNPJ,
                            ClienteNome = cliente?.Nome,
                            NomeDoPlanoDeContas = plano?.Nome
                        });

                        progress.IncrementProcessed();

                        if (batch.Count >= BATCH_SIZE)
                        {
                            await repository.InsertVendasConsolidadasBulkAsync(batch, ct);
                            foreach (var item in batch)
                                _consolidadasInseridas.Add(item.Id);
                            log.Add($"Batch inserido: {batch.Count} itens.");
                            batch.Clear();
                        }
                    }
                }


                watch.Stop();
                log.Add($"Consolidação concluída em {watch.Elapsed}.");
            }
            catch (OperationCanceledException)
            {
                watch.Stop();

                log.Add("Consolidação cancelada pelo usuário.");

                try
                {

                    var coll = repository.GetVendasConsolidadasCollection();

                    if (_consolidadasInseridas.Any())
                    {
                        var filter = Builders<VendaConsolidada>.Filter.In(x => x.Id, _consolidadasInseridas);
                        var result = await coll.DeleteManyAsync(filter);

                        log.Add($"{result.DeletedCount} vendas consolidadas removidas após cancelamento.");
                    }
                    else
                    {
                        log.Add("Nenhuma venda consolidada havia sido inserida para remover.");
                    }
                }
                catch (Exception ex)
                {
                    log.Add("Erro ao remover vendas consolidadas após cancelamento.");
                }


                progress.SetTotal(0);
                progress.ResetProcessed();
                log.Add("Progresso resetado para zero.");

                _consolidadasInseridas.Clear();
            }

            catch (Exception ex)
            {
                watch.Stop();
                log.Add($"Erro: {ex.Message}");
                log.Add( "Erro durante consolidação");
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
        public long GetTotal() => progress.Total;

        public long GetProcessed() => progress.Processed;
    }
        
}
