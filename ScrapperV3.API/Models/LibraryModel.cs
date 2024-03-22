using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ScrapperV3.API.Models
{
    public class LibraryModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public List<Episode>? Episode { get; set; }
    }

    public class Episode
    {
        public string EpisodeNo { get; set; } = string.Empty;
        public List<string>? URL { get; set; }
    }
}
