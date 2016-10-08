using System.IO;

namespace FactorioModManager.Lib.Archive
{
    public interface IArchiveEntry
    {
        string RelativePath { get; }
        bool IsFolder { get; }
        void Extract(Stream readStream);
    }
}
