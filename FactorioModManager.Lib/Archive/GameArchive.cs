using System;
using System.Collections.Generic;
using System.IO;
using FactorioModManager.Lib.Web;
using OperatingSystem = FactorioModManager.Lib.Models.OperatingSystem;

namespace FactorioModManager.Lib.Archive
{
    public class GameArchive : IDisposable
    {
        private readonly IGameArchiveReader _impl;
        
        public GameArchiveSpec Spec { get; }

        public GameArchive(Stream readStream, GameArchiveSpec spec)
        {
            if (readStream == null)
                throw new ArgumentNullException("readStream");
            if (spec == null)
                throw new ArgumentNullException("spec");

            Spec = spec;

            switch (spec.OperatingSystem)
            {
                case OperatingSystem.Windows:
                    _impl = new GameZipArchiveReader(readStream);
                    break;
                case OperatingSystem.Mac:
                    _impl = new GameDmgArchiveReader(readStream);
                    break;
                case OperatingSystem.Linux:
                    _impl = new GameTgzArchiveReader(readStream);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("spec");
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
