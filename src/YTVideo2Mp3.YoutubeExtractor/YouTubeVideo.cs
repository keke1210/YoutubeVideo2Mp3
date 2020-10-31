using System;
using System.Threading.Tasks;
using YTVideoToMp3.Extractor.Helpers;

namespace YTVideoToMp3.Extractor
{
    public partial class YouTubeVideo : Video
    {
        private readonly string jsPlayer;

        private string uri;
        private Query uriQuery;
        private bool encrypted;
        public YouTubeVideo(string title, UnscrambledQuery query, string jsPlayer)
        {
            this.Title = title;
            this.uri = query.Uri;
            this.uriQuery = new Query(uri);
            this.jsPlayer = jsPlayer;
            this.encrypted = query.IsEncrypted;
            this.FormatCode = int.Parse(uriQuery["itag"]);
        }
        public override string Title { get; }
        public override WebSites WebSite => WebSites.YouTube;

        public override string Uri => GetUriAsync().GetAwaiter().GetResult();

        public string GetUri(Func<DelegatingClient> makeClient) => GetUriAsync(makeClient).GetAwaiter().GetResult();

        public override Task<string> GetUriAsync() => GetUriAsync(() => new DelegatingClient());

        public async Task<string> GetUriAsync(Func<DelegatingClient> makeClient)
        {
            if (encrypted)
            {
                uri = await
                    DecryptAsync(uri, makeClient)
                    .ConfigureAwait(false);
                encrypted = false;
            }

            return uri;
        }

        public int FormatCode { get; }

        public long? ContentLength
        {
            get
            {
                if (_contentLength.HasValue)
                    return _contentLength;
                _contentLength = this.GetContentLength(uriQuery).Result;
                return _contentLength;
            }
        }

        public bool IsEncrypted => encrypted;

        // Private's
        private long? _contentLength { get; set; }
        private async Task<long?> GetContentLength(Query query)
        {
            if (query.TryGetValue("clen", out string clen))
            {
                return long.Parse(clen);
            }
            using (var client = new VideoClient())
            {
                return await client.GetContentLengthAsync(uri);
            }
        }

    }
}
