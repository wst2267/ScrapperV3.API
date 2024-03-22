namespace ScrapperV3.API.Utility.Configuration
{
    public class MongoDbConfig
    {
        public string ConnectionString { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string LibraryCollection { get; set; } = null!;
    }
}
