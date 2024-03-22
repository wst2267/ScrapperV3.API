using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ScrapperV3.API.Models;
using ScrapperV3.API.Utility.Configuration;
using static System.Collections.Specialized.BitVector32;

namespace ScrapperV3.API.Repository
{
    public interface IScrapperRepository
    {
        Task<List<string>> GetImageOvergear(string section);
        Task InsertNewEpisode(string section, string name, List<string> episodeUrl);
        Task<int> CheckEP();
        Task UpdateNewEpisode(string section, string name, List<string> episodeUrl);
    }

    public class ScrapperRepository : IScrapperRepository
    {
        private readonly LibraryConfig _libraryConfig;
        private readonly IMongoCollection<LibraryModel> _libraryCollection;
        public ScrapperRepository(IOptions<LibraryConfig> libraryConfig, IOptions<MongoDbConfig> dbConfig)
        {
            _libraryConfig = libraryConfig.Value;
            var connection = new MongoClient(dbConfig.Value.ConnectionString);
            var db = connection.GetDatabase(dbConfig.Value.DatabaseName);
            _libraryCollection = db.GetCollection<LibraryModel>(dbConfig.Value.LibraryCollection);
        }

        public async Task<List<string>> GetImageOvergear(string section)
        {
            var listImg = new List<string>();
            var web = new HtmlWeb();
            try
            {
                ValidateSection(section);

                var document = await web.LoadFromWebAsync($"{_libraryConfig.HostRanker}/{string.Format(_libraryConfig.OverGearURL, section)}/");
                var listOverGear = document.DocumentNode.QuerySelectorAll("div.reader-area");

                foreach (var childNode in listOverGear[0].ChildNodes)
                {
                    if (childNode.NodeType != HtmlNodeType.Text)
                    {
                        var nodeImg = HtmlNode.CreateNode(childNode.OuterHtml);
                        var imageURL = nodeImg.Attributes["src"].Value;
                        if (!string.IsNullOrEmpty(imageURL))
                        {
                            listImg.Add(imageURL);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listImg;
        }

        private void ValidateSection(string section)
        {
            if (string.IsNullOrEmpty(section))
            {
                throw new Exception("Please input section");
            }

            var isNumeric = int.TryParse(section, out _);
            if (!isNumeric)
            {
                throw new Exception("Please enter number only");
            }
        }

        public async Task InsertNewEpisode(string section, string name, List<string> episodeUrl)
        {
            var data = new LibraryModel()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Name = name,
                Episode = new List<Episode>
                {
                    new Episode { EpisodeNo = section.ToString(), URL = episodeUrl }
                }
            };

            await _libraryCollection.InsertOneAsync(data);
        }

        public async Task UpdateNewEpisode(string section, string name, List<string> episodeUrl)
        {
            var episode = new Episode { EpisodeNo = section.ToString(), URL = episodeUrl };

            var filter = Builders<LibraryModel>.Filter.Eq("Name", name);
            var update = Builders<LibraryModel>.Update.Push(e => e.Episode, episode);

            await _libraryCollection.UpdateOneAsync(filter, update);
        }

        public async Task<int> CheckEP()
        {
            int countEP = 0;
            var web = new HtmlWeb();
            try
            {
                var document = await web.LoadFromWebAsync($"{_libraryConfig.HostRanker}/series/overgeared-remake/");
                var listOverGear = document.DocumentNode.QuerySelectorAll("ul.series-chapterlist");

                countEP = listOverGear[0].ChildNodes.Count(o => o.NodeType == HtmlNodeType.Element);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return countEP;
        }
    }
}
