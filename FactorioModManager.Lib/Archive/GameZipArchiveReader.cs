using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using FactorioModManager.Lib.Files;

namespace FactorioModManager.Lib.Archive
{
    class GameZipArchiveReader : IGameArchiveReader
    {
        /// <exception cref="ArgumentNullException"><paramref name="archiveFile"/> or <paramref name="outputDir"/> is <see langword="null" />.</exception>
        /// <exception cref="FailedToLaunchSevenZipException">The 7Zip executable could not be found or failed to launch.</exception>
        /// <exception cref="SevenZipExitCodeException">Seven zip returned with a bad exit code.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="InvalidGameArchiveContentsException"></exception>
        /// <exception cref="ArchiveException"></exception>
        public async Task ExtractToDir(string archiveFile, string outputDir)
        {
            if (archiveFile == null)
                throw new ArgumentNullException("archiveFile");
            if (outputDir == null)
                throw new ArgumentNullException("outputDir");

            // The contents of the archive is entirely contained in a root folder called "Factorio_{VersionNumber x.x.x}".
            // Extract to a temp dir, then recursively copy to the output dir. Strip this folder name from the path during the copy.
            // For example: the path Factorio_0.13.20/bin/x64/factorio.exe becomes bin/x64/factorio.exe

            string tempDir;
            try
            {
                tempDir = FileUtils.GetTempDir();
            }
            catch (IOException ex)
            {
                throw new ArchiveException(ex.Message, ex);
            }

            try
            {
                await SevenZip.ExtractArchive(archiveFile, tempDir);

                var directories = Directory.EnumerateDirectories(tempDir).ToList();
                if (directories.Count != 1)
                    throw new InvalidGameArchiveContentsException();

                var innerTempFolder = directories[0];
                FileUtils.MoveDirectoryAcrossVolume(innerTempFolder, outputDir);
            }
            catch (IOException ex)
            {
                throw new ArchiveException(ex.Message, ex);
            }
            catch (SecurityException ex)
            {
                throw new UnauthorizedAccessException(ex.Message, ex);
            }
            finally
            {
                if (tempDir != null)
                    Directory.Delete(tempDir, recursive: true);
            }
        }
    }
}
