using System;
using System.Collections.Generic;

namespace FactorioModManager.Lib.Archive
{
    interface IFactorioArchive : IDisposable
    {
        IEnumerable<IArchiveEntry> Entries();
    }
}
