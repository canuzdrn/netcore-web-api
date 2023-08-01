using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using userMS.Domain.Entities.Common;

namespace userMS.Domain.Entities
{
    public class User : IEntity<string>
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public DateTime CreatedAt => DateTime.Now;

        public string CreatedBy { get; }

        public User()
        {
            // ObjectId -> logic will change, want id generation to be database agnostic
            Id = ObjectId.GenerateNewId().ToString();

            // createdby logic will change (think as if it is a placeholder for now)
            CreatedBy = Id;
        }
    }
}
