using MongoDB.Driver;
using userMS.Models;

namespace userMS.Repository
{
    public interface IGenericRepository<TDocument> where TDocument : IBaseDocument
    {
        IEnumerable<TDocument> GetAll();
        TDocument GetById(string id);
        TDocument Create(TDocument document);
        TDocument Update(string id, TDocument document);
        void Delete(string id);
    }
}
