using Amazon.Runtime.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using userMS.Data;
using userMS.Models;

namespace userMS.Repository
{
    public class GenericRepository<TDocument> : IGenericRepository<TDocument> where TDocument : IBaseDocument
    {
        private readonly IOptions<DatabaseSettings> _dbSettings;
        private readonly IMongoCollection<TDocument> _collection;

        public GenericRepository(IOptions<DatabaseSettings> dbSettings)
        {
            _dbSettings = dbSettings;

            // mongo client is responsible for managing the connection to the MongoDB server
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _collection = mongoDatabase.GetCollection<TDocument>(typeof(TDocument).Name + "Collection");
        }

        public TDocument Create(TDocument document)
        {
            _collection.InsertOne(document);
            return document;
        }

        public void Delete(string id)
        {
            _collection.DeleteOne(d => d.Id == id);
        }

        public IEnumerable<TDocument> GetAll()
        {
            return _collection.Find(_ => true).ToList();
        }

        public TDocument GetById(string id)
        {
            return _collection.Find(d => d.Id == id).FirstOrDefault();
        }

        public TDocument Update(string id, TDocument document)
        {
            _collection.ReplaceOne(d => d.Id == id, document);
            return document;
        }
    }
}
