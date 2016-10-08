using System;
using System.IO;
using SevenZipEntry = SevenZipExtractor.Entry;

namespace FactorioModManager.Lib.Archive
{
    class SevenZipEntryToArchiveEntryAdapter : IArchiveEntry
    {
        private readonly SevenZipEntry _impl;

        public SevenZipEntryToArchiveEntryAdapter(SevenZipEntry impl, bool isFolder, string relativePath)
        {
            if (impl == null)
                throw new ArgumentNullException("impl");

            _impl = impl;
            IsFolder = isFolder;
            RelativePath = relativePath;
        }

        public string RelativePath { get; }
        public bool IsFolder { get; }

        public void Extract(Stream readStream)
        {
            if (readStream == null)
                throw new ArgumentNullException("readStream");
            if (!readStream.CanRead)
                throw new ArgumentException("The stream must be readable.", "readStream");

            _impl.Extract(readStream);
        }
    }
}
