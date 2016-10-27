using System;
using System.Threading.Tasks;

namespace FactorioModManager.Lib.Archive
{
    interface IGameArchiveReader
    {
        /// <exception cref="ArgumentNullException"><paramref name="archiveFile"/> or <paramref name="outputDir"/> is <see langword="null" />.</exception>
        /// <exception cref="FailedToLaunchSevenZipException">The 7Zip executable could not be found or failed to launch.</exception>
        /// <exception cref="SevenZipExitCodeException">Seven zip returned with a bad exit code.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="InvalidGameArchiveContentsException"></exception>
        /// <exception cref="ArchiveException"></exception>
        Task ExtractToDir(string archiveFile, string outputDir);
    }
}
