using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
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

        /// <summary>
        /// Note: the path extension may change during this operation based on the archive format received.
        /// </summary>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="spec"/> or <paramref name="destinationPath"/> is <see langword="null" />.</exception>
        public async Task<GameArchive> GetGameAsArchive(GameArchiveSpec spec, string destinationPath)
        {
            if (spec == null)
                throw new ArgumentNullException("spec");
            if (destinationPath == null)
                throw new ArgumentNullException("destinationPath");

            // Append the proper extension if it doesn't have it already
            var extension = spec.GetFileExtension();
            if (!destinationPath.EndsWith(extension))
                destinationPath = destinationPath + extension;

            using (var outStream = new FileStream(destinationPath, FileMode.OpenOrCreate))
            using (var inStream = await GetGameAsArchiveStream(spec))
            {
                await inStream.CopyToAsync(outStream);
            }

            return new GameArchive(destinationPath, spec);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}