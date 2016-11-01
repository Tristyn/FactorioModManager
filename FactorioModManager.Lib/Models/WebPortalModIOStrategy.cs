using System;
using System.IO;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace FactorioModManager.Lib.Models
{
    /// <summary>
    /// A mod IO strategy that fulfills it's requests by using the official Factorio mod portal.
    /// </summary>
    class WebPortalModIOStrategy : ModIOStrategy
    {
        private readonly ModDownloadSpec _spec;
        private readonly AsyncLock _cacheLock = new AsyncLock();

        /// <exception cref="ArgumentNullException"><paramref name="spec"/> is <see langword="null" />.</exception>
        public WebPortalModIOStrategy(ModDownloadSpec spec)
        {
            if (spec == null)
                throw new ArgumentNullException("spec");

            _spec = spec;
        }

        public override Task<bool> Exists()
        {
            throw new NotImplementedException();
        }

        public override Task CopyTo(string path)
        {
            throw new NotImplementedException();
        }

        public override Task<ModMetadata> GetMetadata()
        {
            throw new NotImplementedException();
        }

        public override Task<Stream> ToStream()
        {
            throw new NotImplementedException();
        }
    }
}
