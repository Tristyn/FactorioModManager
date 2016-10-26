using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using FactorioModManager.Lib.Files;
using SevenZipExtractor;

namespace FactorioModManager.Lib.Archive
{
    class GameZipArchiveReader : IGameArchiveReader
    {
        private const string ArchiveRootDirectoryNamePrefix = "Factorio_";
        
        public GameZipArchiveReader()
        {

        }

        /// <exception cref="ArgumentNullException"><paramref name="archiveFile"/> or <paramref name="outputDir"/> is <see langword="null" />.</exception>
        /// <exception cref="IllegalPathCharactersException">The archive or extraction path contained illegal characters.</exception>
        /// <exception cref="FailedToLaunchSevenZipException">The 7Zip executable could not be found or failed to launch.</exception>
        /// <exception cref="SevenZipExitCodeException">Seven zip returned with a bad exit code.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permissions. </exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="path" /> is invalid, such as referring to an unmapped drive. </exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="InvalidGameArchiveContentsException"></exception>
        public async Task ExtractToDir(string archiveFile, string outputDir)
        {
            if (archiveFile == null)
                throw new ArgumentNullException("archiveFile");
            if (outputDir == null)
                throw new ArgumentNullException("outputDir");
            
            // The contents of the archive is entirely contained in a root folder called "Factorio_{VersionNumber x.x.x}".
            // Extract to a temp dir, then recursively copy to the output dir. Strip this folder name from the path during the copy.
            // For example: the path Factorio_0.13.20/bin/x64/factorio.exe becomes bin/x64/factorio.exe

            var tempDir = FileUtils.GetTempDir();

            await SevenZip.ExtractArchive(archiveFile, tempDir);

            try
            {
                var directories = Directory.EnumerateDirectories(tempDir).ToList();
                if (directories.Count != 1)
                    throw new InvalidGameArchiveContentsException();

                var innerTempFolder = directories[0];
                FileUtils.MoveDirectoryAcrossVolume(innerTempFolder, outputDir);
            }
            catch (IOException ex)
            {
                // path is a file name
                throw new DirectoryNotFoundException(ex.Message, ex);
            }
            finally
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }

        public IEnumerable<IArchiveEntry> Entries()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }
    }
}
