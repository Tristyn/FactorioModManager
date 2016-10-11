using System;
using FactorioModManager.Lib.Models;
using OperatingSystem = FactorioModManager.Lib.Models.OperatingSystem;

namespace FactorioModManager.Lib.Web
{
    /// <summary>
    /// Provides well-formed uris for resources at www.factorio.com
    /// </summary>
    class FactorioHomepageUriFactory
    {
        private readonly Uri _homepageUriBase;
        private readonly Uri _gameArchiveDownloadsUriBase;

        private static readonly Uri DefaultHomepageUriBase = new Uri("https://www.factorio.com/");

        
        public FactorioHomepageUriFactory()
            : this(DefaultHomepageUriBase)
        {
            
        }

        /// <param name="homepageUriBase">Override the default homepage uri "https://www.factorio.com/" with another one.</param>
        public FactorioHomepageUriFactory(Uri homepageUriBase)
        {
            if (homepageUriBase == null)
                throw new ArgumentNullException("homepageUriBase");

            _homepageUriBase = homepageUriBase;
            _gameArchiveDownloadsUriBase = new Uri(_homepageUriBase, "get-download/");
        }

        public Uri GetHomepage()
        {
            return _homepageUriBase;
        }

        public Uri GetLoginPage()
        {
            return new Uri(_homepageUriBase, "login");
        }

        /// <summary>
        /// Returns an archive download link that returns a game archive for the specified OperatingSystem, game version, bitness and client type.
        /// </summary>
        public Uri GetGameArchiveDownloadUri(GameArchiveSpec spec)
        {
            /* uri format: factorio.com/get-download/{version}/{build}/{platform}
             * 
             * version: 0.0.0
             * build: alpha, demo, headless (only linux64 platform)
             * platform: 
             * - win64: .exe file, win64-manual: .zip file, win32, .exe file, win32-manual: .zip file
             * - osx: .dmg file
             * - linux64: .tar.gz file, linux32: .tar.gz file
             * 
             */

            string platform;
            switch (spec.OperatingSystem)
            {
                case OperatingSystem.Windows:
                    platform = "win64-manual";
                    break;
                case OperatingSystem.Mac:
                    platform = "osx";
                    break;
                case OperatingSystem.Linux:
                    platform = "linux64";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var uriTail = string.Format("{0}/{1}/{2}", spec.Version, spec.BuildConfiguration, platform);
            return new Uri(_gameArchiveDownloadsUriBase, uriTail);
        }

        public Uri GetGameArchiveFeedPageUri(bool isExperimentalFeed, bool onlyHeadlessServerFeed)
        {
            // https://www.factorio.com/[download | download-headless]/[experimental?]

            var uriTail = string.Format("{0}/{1}", 
                onlyHeadlessServerFeed ? "download-headless" : "download",
                isExperimentalFeed ? "experimental" : "");

            return new Uri(_homepageUriBase, uriTail);
        }
    }
}
