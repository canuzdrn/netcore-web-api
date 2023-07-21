using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace userMS.Models
{
    public class User : IBaseDocument
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public DateTime CreatedAt => DateTime.Now;

        public User()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }
    }

    
}
