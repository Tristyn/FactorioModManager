using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SevenZipExtractor;

namespace FactorioModManager.Lib.Archive
{
    class FactorioTgzArchive : IFactorioArchive
    {
        private const string ArchiveRootDirectoryName = "factorio";
        
        private readonly ArchiveFile _archive;

        public FactorioTgzArchive(Stream readStream)
        {
            if (readStream == null)
                throw new ArgumentNullException("readStream");
            if (!readStream.CanRead)
                throw new ArgumentException("The stream must be readable.", "readStream");

            // From the stream extract a GZip archive.
            // From that, extract the Tar archive.

            var gzip = new ArchiveFile(readStream, KnownSevenZipFormat.GZip);
            var tarStream = new MemoryStream();
            gzip.Entries.First().Extract(tarStream);
            _archive = new ArchiveFile(tarStream, KnownSevenZipFormat.Tar);
        }

        public IEnumerable<IArchiveEntry> Entries()
        {
            // The contents of the archive is entirely contained in a root folder called "factorio".
            // Strip this folder name from the base of all paths before yielding each entry.
            // For example: the path factorio/bin/x64/factorio becomes bin/x64/factorio
            
            foreach (var entry in _archive.Entries)
            {
                var fileRelativePath = entry.FileName;
                Debug.Assert(fileRelativePath.StartsWith(ArchiveRootDirectoryName));

                var trimmedFileRelativePath = fileRelativePath
                    .Substring(ArchiveRootDirectoryName.Length + 1);
                // Exclude 1 extra character from the substring
                // so that the directory seperator character is also trimmed from the string.
                
                // Exclude the root folder, as we are trying to hide it's existence.
                // The root folder path is an empty string after the trim operation.
                if (string.IsNullOrWhiteSpace(trimmedFileRelativePath))
                    continue;

                yield return new SevenZipEntryToArchiveEntryAdapter(entry, entry.IsFolder, trimmedFileRelativePath);
            }
        }

        public void Dispose()
        {
            _archive.Dispose();
        }
    }
}
