using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FactorioModManager.Lib.Archive;
using FactorioModManager.Lib.Contracts;

namespace FactorioModManager.Lib.Web
{
    public class FactorioWebClient : IModPortalClient
    {
        public bool Authorize(string username, string password)
        {
            /* 
             * url: factorio.com/login
             * method: post
             * request content type: application/x-www-form-urlencoded
             * 
             *  == form fields: ==
             * csrf_token: string scraped from get: factorio.com/login form
             * username_or_email: string
             * password: string
             * action: string = "Login"
             */
            throw new NotImplementedException();
        }
    }

    public class AuthorizedFactorioWebClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly FactorioHomepageUriFactory _homepageUriFactory
            = new FactorioHomepageUriFactory();

        public AuthorizedFactorioWebClient(string session)
        {
            // Add the session cookie, which acts as authorization for Factorio APIs
            var cookies = new CookieContainer();
            cookies.Add(new Cookie("session", session, "/", "https://www.factorio.com/"));

            // set up HttpClient
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.CookieContainer = cookies;
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
