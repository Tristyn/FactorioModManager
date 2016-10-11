using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FactorioModManager.Lib.Archive;

namespace FactorioModManager.Lib.Web
{
    public class AuthorizedFactorioWebClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly FactorioHomepageUriFactory _homepageUriFactory
            = new FactorioHomepageUriFactory();

        public AuthorizedFactorioWebClient(FactorioAuthToken authToken)
        {
            // Add the session cookie, which acts as authorization for Factorio APIs
            var cookies = new CookieContainer();
            cookies.Add(authToken.ToCookie());

            // set up HttpClient
            var httpClientHandler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = cookies
            };
            _httpClient = new HttpClient(httpClientHandler);
        }

        public async Task<GameArchiveFeed> GetGameArchiveFeed()
        {
            // There is a "stable" feed and an "experimental" feed.
            // Grab both and aggregate them.

            var feedUris = new[]
            {
                _homepageUriFactory.GetGameArchiveFeedPageUri(true, false),
                _homepageUriFactory.GetGameArchiveFeedPageUri(false, false)
            };

            var downloadTasks = feedUris.Select(_httpClient.GetStringAsync);
            var feedHtmls = await Task.WhenAll(downloadTasks);

            var feeds = feedHtmls.Select(GameArchiveFeed.FromFactorioDownloadsPageHtml);

            // Aggregate the feeds and order by version number
            var aggregateFeedEntries = feeds
                // This SelectMany merges both feed lists into one
                .SelectMany(feed => feed)
                .OrderByDescending(downloadSpec => downloadSpec.Version)
                .ToList();

            return new GameArchiveFeed(aggregateFeedEntries);
        }

        public async Task<Stream> GetGameAsArchiveStream(GameArchiveSpec spec)
        {
            var uri = _homepageUriFactory.GetGameArchiveDownloadUri(spec);
            return await _httpClient.GetStreamAsync(uri);
        }

        public async Task<GameArchive> GetGameAsArchive(GameArchiveSpec spec)
        {
            var stream = await GetGameAsArchiveStream(spec);
            return new GameArchive(stream, spec);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}