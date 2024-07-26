using LoanGeteway.Common;
using LoanGeteway.Models;
using MongoDB.Driver;

namespace LoanGateway.Services
{
    public class MongoHelper: IMongoHelper
    {
        private readonly IMongoClient client;
        private readonly IMongoDatabase db;
        public MongoHelper(IConfiguration config)
        {
            var settings = MongoClientSettings.FromConnectionString(config.GetConnectionString(DbObjects.Uri));
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            client = new MongoClient(settings);
            db = client.GetDatabase(DbObjects.DbName);
        }

        public IMongoDatabase GetDb() { return db; }

    }
}
