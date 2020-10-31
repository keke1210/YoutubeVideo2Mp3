using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using YTVideoToMp3.Services;
using YTVideoToMp3.ViewModels;

namespace YTVideoToMp3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IYoutubeService _youtubeService;

        [BindProperty]
        public VideoViewModel Video { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IYoutubeService youtubeService)
        {
            _logger = logger;
            _youtubeService = youtubeService;
        }

        public void OnGet()
        {
        }

        public async Task OnPostAsync()
        {
            var video = await _youtubeService.GetVideoAsync("https://www.youtube.com/watch?v=a7BvGBT0gFw&list=PLOeFnOV9YBa6RxHlG61T8u0ApgPaelszF");
            
        }
    }
}
