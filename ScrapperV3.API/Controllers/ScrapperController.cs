using Microsoft.AspNetCore.Mvc;
using ScrapperV3.API.Repository;

namespace ScrapperV3.API.Controllers
{
    [Route("api/Scrapper")]
    [ApiController]
    public class ScrapperController : ControllerBase
    {
        private readonly IScrapperRepository _scrapperRepository;
        public ScrapperController(IScrapperRepository scrapperRepository)
        {
            _scrapperRepository = scrapperRepository;
        }

        [HttpGet]
        [Route("GetImgOvergear")]
        public async Task<List<string>> GetImgOvergear(string section)
        {
            var listImg = await _scrapperRepository.GetImageOvergear(section);
            if (listImg.Any())
            {
                await _scrapperRepository.UpdateNewEpisode(section, "overgear", listImg);
            }
            return listImg;
        }

        [HttpPost]
        [Route("CheckEP")]
        public async Task<int> CheckEP()
        {
            var countEP = await _scrapperRepository.CheckEP();
            return countEP;
        }
    }
}
