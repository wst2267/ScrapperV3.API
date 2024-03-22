using HtmlAgilityPack;

namespace ScrapperV3.API.Repository
{
    public interface IScrapperRepository
    {
        Task<List<string>> GetImageOvergear(string section);
    }
    public class ScrapperRepository : IScrapperRepository
    {
        public ScrapperRepository() { }

        public async Task<List<string>> GetImageOvergear(string section)
        {
            var listImg = new List<string>();
            var web = new HtmlWeb();
            try
            {
                ValidateSection(section); 

                var document = web.Load($"https://www.ranker-manga.com/overgeared-remake-{section}/");
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
    }
}
