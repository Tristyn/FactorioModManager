using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SevenZipExtractor;

namespace FactorioModManager.Lib.Archive
{
    class GameDmgArchiveReader : IGameArchiveReader
    {
        private const string ArchiveRootDirectoryName = "factorio";

        private readonly ArchiveFile _archive;

        public GameDmgArchiveReader(Stream readStream)
        {
            if (readStream == null)
                throw new ArgumentNullException("readStream");
            if (!readStream.CanRead)
                throw new ArgumentException("The stream must be readable.", "readStream");

            // Open a factorio .dmg file in 7zip gui to see the beautiful mess that is .dmg images
            var dmgArchive = new ArchiveFile(readStream, KnownSevenZipFormat.Dmg);

            // Select the hfs volume from dmg
            var hfsEntry = dmgArchive.Entries.Single(entry => entry.FileName.EndsWith(".hfs"));

            // Dump the hfs volume into a temp file. As of Factorio
            // version 0.13.20 it takes about 400MB unpacked
            var hfsStream = new TempFileStream(FileAccess.ReadWrite, FileShare.None, 1024 * 1024);
            hfsEntry.Extract(hfsStream);
            // Seek to the beginning of the stream so that it can be read again.
            hfsStream.Seek(0, SeekOrigin.Begin);

            _archive = new ArchiveFile(hfsStream, KnownSevenZipFormat.Hfs);
        }

        public IEnumerable<IArchiveEntry> Entries()
        {
            // The contents of the archive is entirely contained in a root folder called "factorio".
            // Strip this folder name from the base of all paths before yielding each entry.
            // For example: the path factorio/.DS_Store becomes .DS_Store

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
