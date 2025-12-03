using MongoDB.Driver;

namespace ConsolidacaoVendas.Mongo
{
        public class MongoSettings
        {
            public string OrigemConnectionString { get; set; } = null!;
            public string OrigemDatabase { get; set; } = null!;
            public string DestinoConnectionString { get; set; } = null!;
            public string DestinoDatabase { get; set; } = null!;
        }

        public class MongoContext
        {
            public IMongoDatabase OrigemDb { get; }
            public IMongoDatabase DestinoDb { get; }

            public MongoContext(MongoSettings settings)
            {
                var origemClient = new MongoClient(settings.OrigemConnectionString);
                OrigemDb = origemClient.GetDatabase(settings.OrigemDatabase);

                var destinoClient = new MongoClient(settings.DestinoConnectionString);
                DestinoDb = destinoClient.GetDatabase(settings.DestinoDatabase);
            }
        }
    }

