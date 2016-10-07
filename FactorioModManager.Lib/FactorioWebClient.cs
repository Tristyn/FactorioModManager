using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FactorioModManager.Lib.Contracts;
using FactorioModManager.Lib.Models;
using SevenZipExtractor;
using ZipArchive = SharpCompress.Archives.Zip.ZipArchive;
using TarArchive = SharpCompress.Archives.Tar.TarArchive;

namespace FactorioModManager.Lib
{
    public class FactorioWebClient : IModPortalClient
    {
        static internal Uri GameBaseUri = new Uri("https://www.factorio.com/");
        /*
Game downloading notes:
factorio.com/get-download/{version}/{build}/{platform}

build: alpha, demo, headless (only linux64 platform)

platform: win64: .exe, win64-manual: .zip, win32, .exe win32-manual: .zip, osx: .dmg, linux64: .tar.gz, linux32: .tar.gz,



factorio package feeds
/download: stable release
/download/experimental: experimental release

query body > div.container > h3 to scrape version numbers
         */

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
        static internal Uri GameDownloadsUri = new Uri(FactorioWebClient.GameBaseUri, "get-download/");

        private readonly HttpClient _httpClient;

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

        public async Task<ArchiveFile> DownloadInstallationArchive(GameDownloadSpec spec)
        {
            var httpResponse = await _httpClient.GetAsync(GameDownloadsUri);
            var inStream = await httpResponse.Content.ReadAsStreamAsync();
            
            /* The stream has a different format depending on operating system
             * Windows: .zip
             * Linux: .tar.gz
             * Mac: .dmg (HFS+ image)
             */

            switch (spec.OS)
            {
                case OS.Windows:
                    return new ArchiveFile(inStream, KnownSevenZipFormat.Zip);
                case OS.Linux:

                    // From the stream extract a GZip archive.
                    // From that, extract the tar archive.

                    var gzip = new ArchiveFile(inStream, KnownSevenZipFormat.GZip);
                    var tarStream = new MemoryStream();
                    gzip.Entries.First().Extract(tarStream);
                    return new ArchiveFile(tarStream, KnownSevenZipFormat.Tar);
                    
                case OS.Mac:
                    return new ArchiveFile(inStream, KnownSevenZipFormat.Dmg);
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
