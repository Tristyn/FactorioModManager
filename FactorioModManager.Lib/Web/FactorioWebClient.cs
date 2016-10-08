using System;
using System.IO;
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

        public async Task<Stream> GetGameAsArchiveStream(GameDownloadSpec spec)
        {
            var uri = _homepageUriFactory.GetGameArchiveDownloadUri(spec);
            var httpResponse = await _httpClient.GetAsync(uri);
            return await httpResponse.Content.ReadAsStreamAsync();
        }

        public async Task<FactorioArchiveReader> GetGameAsArchive(GameDownloadSpec spec)
        {
            var stream = await GetGameAsArchiveStream(spec);
            return new FactorioArchiveReader(stream, spec.OS);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
