using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SevenZipExtractor;

namespace FactorioModManager.Lib.Archive
{
    class FactorioZipArchive : IFactorioArchive
    {
        private const string ArchiveRootDirectoryNamePrefix = "Factorio_";

        private readonly ArchiveFile _archive;

        public FactorioZipArchive(Stream readStream)
        {
            if (readStream == null)
                throw new ArgumentNullException("readStream");
            if (!readStream.CanRead)
                throw new ArgumentException("The stream must be readable.", "readStream");

            _archive = new ArchiveFile(readStream, KnownSevenZipFormat.Zip);
        }

        private string DetectRootDirectoryName()
        {
            // All entries will contain the directory
            // name, so just grab the first entry
            var filePath = _archive.Entries.First().FileName;

            Debug.Assert(filePath.StartsWith(ArchiveRootDirectoryNamePrefix));

            // A zip files directory seperator character is
            // implementation dependent, so split using both.
            return filePath.Split('/', '\\').First();
        }
        
        public IEnumerable<IArchiveEntry> Entries()
        {
            // The contents of the archive is entirely contained in a root folder called "Factorio_{VersionNumber x.x.x}".
            // Strip this folder name from the base of all paths before yielding each entry.
            // For example: the path Factorio_0.13.20/bin/x64/factorio.exe becomes bin/x64/factorio.exe

            var archiveRootDirectoryName = DetectRootDirectoryName();

            foreach (var entry in _archive.Entries)
            {
                var fileRelativePath = entry.FileName;
                Debug.Assert(fileRelativePath.StartsWith(archiveRootDirectoryName));
                
                var trimmedFileRelativePath = fileRelativePath
                    .Substring(archiveRootDirectoryName.Length + 1);
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
