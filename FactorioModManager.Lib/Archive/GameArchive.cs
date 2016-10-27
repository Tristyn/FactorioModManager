using System;
using System.Threading.Tasks;
using FactorioModManager.Lib.Web;
using OperatingSystem = FactorioModManager.Lib.Models.OperatingSystem;

namespace FactorioModManager.Lib.Archive
{
    public class GameArchive
    {
        private readonly IGameArchiveReader _impl;

        public GameArchiveSpec Spec { get; }

        public string FilePath { get; }

        /// <exception cref="ArgumentNullException"><paramref name="filePath"/> or <paramref name="spec"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">spec</exception>
        public GameArchive(string filePath, GameArchiveSpec spec)
        {
            FilePath = filePath;
            if (filePath == null)
                throw new ArgumentNullException("readStream");
            if (spec == null)
                throw new ArgumentNullException("spec");

            Spec = spec;

            switch (spec.OperatingSystem)
            {
                case OperatingSystem.Windows:
                    _impl = new GameZipArchiveReader();
                    break;
                case OperatingSystem.Mac:
                    _impl = new GameDmgArchiveReader();
                    break;
                case OperatingSystem.Linux:
                    _impl = new GameTgzArchiveReader();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("spec");
            }
        }

        /// <summary>
        /// Extracts the contents of the archive to the specified directory.
        /// </summary>
        /// <param name="destinationDirectory">The folder to extract to. The directory will be created if it does not exist.</param>
        /// <exception cref="SevenZipExitCodeException">Seven zip returned with a bad exit code.</exception>
        /// <exception cref="FailedToLaunchSevenZipException">The 7Zip executable could not be found or failed to launch.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="InvalidGameArchiveContentsException"></exception>
        /// <exception cref="ArchiveException"></exception>
        /// <exception cref="ArgumentNullException"><paramref name="destinationDirectory"/> is <see langword="null" />.</exception>
        public async Task Extract(string destinationDirectory)
        {
            if (destinationDirectory == null)
                throw new ArgumentNullException("destinationDirectory");
            await _impl.ExtractToDir(FilePath, destinationDirectory);
        }
    }
}
