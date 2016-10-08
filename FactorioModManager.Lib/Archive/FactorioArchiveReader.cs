using System;
using System.Collections.Generic;
using System.IO;
using FactorioModManager.Lib.Models;

namespace FactorioModManager.Lib.Archive
{
    public class FactorioArchiveReader : IDisposable
    {
        private readonly IFactorioArchive _impl;

        public FactorioArchiveReader(Stream readStream, OS archivePlatform)
        {
            if (readStream == null)
                throw new ArgumentNullException("readStream");
            
            switch (archivePlatform)
            {
                case OS.Windows:
                    _impl = new FactorioZipArchive(readStream);
                    break;
                case OS.Mac:
                    _impl = new FactorioDmgArchive(readStream);
                    break;
                case OS.Linux:
                    _impl = new FactorioTgzArchive(readStream);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("archivePlatform");
            }
        }

        public IEnumerable<IArchiveEntry> Entries()
        {
            return _impl.Entries();
        }

        public void Dispose()
        {
            _impl.Dispose();
        }
    }
}
