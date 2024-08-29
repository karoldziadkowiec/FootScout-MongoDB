using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FootScout_MongoDB.WebAPI.Entities
{
    public class User
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("PasswordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("FirstName")]
        public string FirstName { get; set; }

        [BsonElement("LastName")]
        public string LastName { get; set; }

        [BsonElement("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [BsonElement("Location")]
        public string Location { get; set; }

        [BsonElement("CreationDate")]
        public DateTime CreationDate { get; set; }
    }
}