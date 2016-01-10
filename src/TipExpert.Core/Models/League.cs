using MongoDB.Bson;

namespace TipExpert.Core
{
    public class League
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
    }
}