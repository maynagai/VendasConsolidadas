namespace ConsolidacaoVendas.Models
{
    public class Venda
    {
        public string? Id { get; set; }
        public decimal? Valor { get; set; }
        public DateTime? Data { get; set; }
        public string? EmpresaId { get; set; }
        public string? PlanoDeContaId { get; set; }
    }
}
