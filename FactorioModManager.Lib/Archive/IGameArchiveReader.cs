using System;
using System.Collections.Generic;

namespace FactorioModManager.Lib.Archive
{
    interface IGameArchiveReader : IDisposable
    {
        IEnumerable<IArchiveEntry> Entries();
    }
}
