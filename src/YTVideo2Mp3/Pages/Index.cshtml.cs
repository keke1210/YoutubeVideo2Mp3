using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using YTVideoToMp3.Services;
using YTVideoToMp3.ViewModels;

namespace YTVideoToMp3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IYoutubeService _youtubeService;
        private readonly IMemoryCache _cache;

        private const string _mp3StringFileNameFormat = "{0}.mp3";
        private const string _videoContentCacheKeyFormat = "VideoContent_{0}";
        private const string _titleCacheKey = "VideoTitle";

        [BindProperty]
        public VideoViewModel Video { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IYoutubeService youtubeService, IMemoryCache cache)
        {
            _logger = logger;
            _youtubeService = youtubeService;
            _cache = cache;
        }

        public void OnGet()
        {
            Video = new VideoViewModel();
        }

        public async Task<IActionResult> OnPostConvertAsync(string youtubeUrl)
        {
            try
            {
                var video = await _youtubeService.GetVideoAsync(youtubeUrl);

                var mp3FileName = string.Format(_mp3StringFileNameFormat, video.Title);

                var videoBytes = await video.GetBytesAsync();

                Video = new VideoViewModel
                {
                    VideoIsConverted = true,
                    Title = video.Title,
                };

                var cacheKey = string.Format(_videoContentCacheKeyFormat, video.Title);

                _cache.Set(cacheKey, videoBytes);
                _cache.Set(_titleCacheKey, video.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return null;
        }

        public IActionResult OnPostDownload()
        {
            try
            {
                var videoTitle = _cache.Get<string>(_titleCacheKey);

                var mp3FileName = string.Format(_mp3StringFileNameFormat, videoTitle);

                var cacheKey = string.Format(_videoContentCacheKeyFormat, videoTitle);

                var videoContent = _cache.Get<byte[]>(cacheKey);

                return File(videoContent, "audio/mpeg", mp3FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return null;
        }
    }
}
