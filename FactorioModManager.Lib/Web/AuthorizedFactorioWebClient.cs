using System;
using System.Diagnostics;
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
        private readonly HttpClient _client;
        private readonly FactorioHomepageUriFactory _homepageUriFactory
            = new FactorioHomepageUriFactory();
        private readonly FactorioModPortalUriFactory _modPortalUriFactory
            = new FactorioModPortalUriFactory();

        private readonly CookieContainer _clientCookies;
        private readonly FactorioUserSession _userSession;

        /// <exception cref="ArgumentNullException"><paramref name="userSession"/> is <see langword="null" />.</exception>
        public AuthorizedFactorioWebClient(FactorioUserSession userSession)
        {
            if (userSession == null)
                throw new ArgumentNullException("userSession");

            _userSession = userSession;

            // set up HttpClient with user auth cookies
            _clientCookies = new CookieContainer();
            var httpClientHandler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = _clientCookies
            };
            _client = new HttpClient(httpClientHandler);
        }

        // ReSharper disable ExceptionNotThrown
        /// <exception cref="WebException">Request failed due to a bad connection (e.g. no internet).</exception>
        /// <exception cref="HttpRequestException">Recieved an error status code that could not be recovered from.</exception>
        // ReSharper restore ExceptionNotThrown
        public async Task<GameArchiveFeed> GetGameArchiveFeed()
        {
            // There is a "stable" feed and an "experimental" feed.
            // Grab both and aggregate them.

            var feedUris = new[]
            {
                _homepageUriFactory.GetGameArchiveFeedPageUri(true, false),
                _homepageUriFactory.GetGameArchiveFeedPageUri(false, false)
            };

            var downloadTasks = feedUris.Select(_client.GetStringAsync);
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

        // ReSharper disable ExceptionNotThrown
        /// <exception cref="WebException">Request failed due to a bad connection (e.g. no internet).</exception>
        /// <exception cref="HttpRequestException">Recieved an error status code that could not be recovered from.</exception>
        // ReSharper restore ExceptionNotThrown
        public async Task<Stream> GetGameAsArchiveStream(GameArchiveSpec spec)
        {
            var uri = _homepageUriFactory.GetGameArchiveDownloadUri(spec);
            return await _client.GetStreamAsync(uri);
        }

        /// <summary>
        /// Note: the extension component of the destination path in the GameArchive result may be different based on the archive format received.
        /// </summary>
        /// <exception cref="SecurityException">The caller does not have the required permission to access the file.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="spec"/> or <paramref name="destinationPath"/> is <see langword="null" />.</exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="HttpRequestException">Recieved an error status code that could not be recovered from.</exception>
        public async Task<GameArchive> GetGameAsArchive(GameArchiveSpec spec, string destinationPath)
        {
            if (spec == null)
                throw new ArgumentNullException("spec");
            if (destinationPath == null)
                throw new ArgumentNullException("destinationPath");

            // Append the proper extension if it doesn't have it already
            // So that apps like 7z can properly extract the file in the future
            var extension = spec.GetFileExtension();
            if (!destinationPath.EndsWith(extension))
                destinationPath = destinationPath + extension;

            try
            {
                using (var outStream = new FileStream(destinationPath, FileMode.OpenOrCreate))
                using (var inStream = await GetGameAsArchiveStream(spec))
                {
                    await inStream.CopyToAsync(outStream);
                }
            }

            catch (DirectoryNotFoundException ex)
            {
                throw new FileNotFoundException(ex.Message, ex);
            }

            return new GameArchive(destinationPath, spec);
        }

        /// <summary>
        /// Does an idempotent test to check if the user session is still valid.
        /// </summary>
        // ReSharper disable ExceptionNotThrown
        /// <exception cref="WebException">Request failed due to a bad connection (e.g. no internet).</exception>
        /// <exception cref="HttpRequestException">Recieved an error status code that could not be recovered from.</exception>
        // ReSharper restore ExceptionNotThrown
        public async Task<bool> IsAuthorized()
        {
            var homepageUri = _homepageUriFactory.GetGameArchiveFeedPageUri(false, false);
            var homepageResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, homepageUri), HttpCompletionOption.ResponseHeadersRead);
            homepageResponse.EnsureSuccessStatusCode();
            bool isAuthorizedHomepage;

            // If we get redirected, we must not be authorized.
            // Assumption: any redirect is to ~/login
            if (homepageResponse.StatusCode == HttpStatusCode.OK)
            {
                if (homepageResponse.RequestMessage.RequestUri == homepageUri)
                {
                    isAuthorizedHomepage = true;
                }
                else
                {
                    isAuthorizedHomepage = false;
                }
            }
            else
            {
                throw new HttpRequestException(homepageResponse.StatusCode.ToString());
            }

            if (!isAuthorizedHomepage)
                return false;

            var modsUri = _modPortalUriFactory.GetModPortalHomepageUri();
            var modPortalResult = await _client.SendAsync(new HttpRequestMessage (HttpMethod.Get, modsUri), HttpCompletionOption.ResponseHeadersRead);
            modPortalResult.EnsureSuccessStatusCode();

            bool isAuthorizedModPortal;
            if (modPortalResult.StatusCode == HttpStatusCode.OK)
            {
                if (modPortalResult.RequestMessage.RequestUri == modsUri)
                {
                    isAuthorizedModPortal= true;
                }
                else
                {
                    isAuthorizedModPortal = false;
                }
            }
            else
            {
                throw new HttpRequestException(modPortalResult.StatusCode.ToString());
            }

            return isAuthorizedModPortal;
        }

        // EnsureAuthorized gets new credentials if necessary and reauthorizes

        /// <exception cref="WebException">Request failed due to a bad connection (e.g. no internet).</exception>
        /// <exception cref="HttpRequestException">Recieved an error status code that could not be recovered from.</exception>
        /// <exception cref="InvalidUsernameAndPasswordException"></exception>
        public async Task EnsureAuthorized()
        {
            if (await IsAuthorized())
                return;

            await _userSession.Invalidate();
            var newSessionCookies = await _userSession.GetSessionCookies();
            _clientCookies.Add(newSessionCookies);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}