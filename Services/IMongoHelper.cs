using MongoDB.Driver;

namespace LoanGateway.Services
{
    public interface IMongoHelper
    {
        IMongoDatabase GetDb();
    }
}
