using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsolidacaoVendas.Models
{
    public class PlanoDeConta
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Id")]
        public string ExternalId { get; set; }
        public string? ClienteId { get; set; }
        public string? Nome { get; set; }
    }
}
