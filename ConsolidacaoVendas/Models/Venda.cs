using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsolidacaoVendas.Models
{
    public class Venda
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Id")]
        public string ExternalId { get; set; }

        public decimal Valor { get; set; }

        public DateTime Data { get; set; }


        public string EmpresaId { get; set; }

        public string PlanoDeContaId { get; set; }
    }
}
