using System;
using FactorioModManager.Lib.Models;

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

        /// <summary>
        /// Returns an archive download link that returns a game archive for the specified OS, game version, bitness and client type.
        /// </summary>
        public Uri GetGameArchiveDownloadUri(GameDownloadSpec spec)
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
            switch (spec.OS)
            {
                case OS.Windows:
                    platform = "win64-manual";
                    break;
                case OS.Mac:
                    platform = "osx";
                    break;
                case OS.Linux:
                    platform = "linux64";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new Uri(_gameArchiveDownloadsUriBase, string.Format("{0}/{1}/{2}", spec.Version, spec.Type, platform));
        }

        public Uri GetGameArchiveFeedWebpagePageUri(GameArchiveFeedSpec spec)
        {
            if (spec == null)
                throw new ArgumentNullException("spec");

            // https://www.factorio.com/[download | download-headless]/[experimental?]

            var uriTail = string.Format("{0}/{1}", 
                spec.OnlyHeadlessServer ? "download-headless" : "download",
                spec.AllowExperimental ? "experimental" : "");

            return new Uri(_homepageUriBase, uriTail);
        }
    }
}
