namespace ConsolidacaoVendas.Models
{
    public class VendaConsolidada
    {
        public string? Id { get; set; }
        public string? IdDaVenda { get; set; } 
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string? EmpresaNome { get; set; }
        public string? CnpjDaEmpresa { get; set; }
        public string? ClienteNome { get; set; }
        public string? NomeDoPlanoDeContas { get; set; }
    }
}
