
---
# VendasConsolidadas 🚀

Um serviço de consolidação de vendas desenvolvido em **C#/.NET**, projetado para integrar registros provenientes de múltiplas coleções do MongoDB, aplicar regras de consistência e armazenar tudo em uma coleção final unificada.

Este projeto foi construído com foco em:
- Alto volume de dados (200k+ vendas)
- Logging estruturado
- Acompanhamento de progresso (ProgressTracker)
- Otimização de performance

---


## 📌 Sobre

A aplicação lê vendas da coleção **Vendas**, consulta dados relacionados em **Cliente**, **Empresa** e **PlanoDeConta**, e grava o resultado consolidado na coleção **VendasConsolidadas**.

Ela foi projetada para simular cenários reais de processamento massivo de dados em back-end de integrações internas.


---

## 🛠 Tecnologias

- .NET 8  
- MongoDB Driver  
- ASP.NET Core Web API  
- C# 
- Task Parallel / async-await

---

## 🧱 Arquitetura
```
ConsolidacaoVendas/
├── Controllers/
│ └── VendasConsolidadasController.cs
├── Services/
│ └── VendasConsolidadasService.cs
├── Repositories/
│ ├── VendasConsolidadasRepository.cs
├── Models/
│ ├── Cliente.cs
│ ├── Empresa.cs
│ ├── Venda.cs
│ ├── VendaConsolidada.cs
│ ├── PlanoDeConta.cs
├── Log/
│ ├── LogTracker.cs
│ └── ProgressTracker.cs
│ ├── Empresa.cs
├── Mongo/
│ ├── Mongoontext.cs
├── Program.cs
└── appsettings.json
```
---
## 🚀 Endpoints da API

### ▶ **POST `/consolidacao/start`**
Inicia o processo de transferência e consolidação.

**Resposta:**
```
{ "message": "Processo de consolidação iniciado" }
```
### ▶ POST /consolidacao/cancelar

Cancela o processo em andamento (via CancellationToken).

Exemplo:
```
{ message = "Pedido de cancelamento enviado"  }
```
### ▶ GET /consolidacao/progresso

Retorna o percentual concluído do processo.

Exemplo:
```
{
  "progresso": "7%",
  "total": 100000,
  "processados": 7000
}
```

### ▶ GET /consolidacao/logs

Retorna logs de execução, incluindo:

    Sucessos

    Falhas

    Tempo total de execução

    Ações importantes

Exemplo:
```
{
  " 06/12/2025 12:36:23 - Consolidação iniciada.",
  " 06/12/2025 12:36:23 - Carregando cache em memória...",
  " 06/12/2025 12:36:23 - Carregado: 20 empresas, 15 planos, 10 clientes",
  " 06/12/2025 12:36:26 - Batch inserido: 1000 itens.",
  " 06/12/2025 12:37:37 - Erro: The connection pool is in paused state for server localhost:27017.",
  " 06/12/2025 12:37:37 - Erro durante consolidação"
}
```
### ▶ GET /vendas-consolidadas

Retorna as vendas consolidadas do banco de destino, agrupadas por data e empresa.

Exemplo:
```
[
 {
    "id": "693455a71f0fae33b6ae806f",
    "idDaVenda": "693353a9db94abfd7b90326d",
    "valor": 9601.61,
    "data": "2025-07-28T00:00:00Z",
    "empresaNome": "Empresa 14",
    "cnpjDaEmpresa": "11606989089523",
    "clienteNome": "Cliente 2",
    "nomeDoPlanoDeContas": "Plano de Conta 5"
  },
  {
    "id": "693455a71f0fae33b6ae891b",
    "idDaVenda": "693353aadb94abfd7b903b19",
    "valor": 2837.36,
    "data": "2025-07-28T00:00:00Z",
    "empresaNome": "Empresa 14",
    "cnpjDaEmpresa": "11606989089523",
    "clienteNome": "Cliente 2",
    "nomeDoPlanoDeContas": "Plano de Conta 1"
  }
]
```
  Diferencial implementado: filtros adicionais (dataInicio, dataFim, nomeEmpresa)
