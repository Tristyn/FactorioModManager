using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FactorioModManager.Lib.Web;
using OperatingSystem = FactorioModManager.Lib.Models.OperatingSystem;

namespace FactorioModManager.Lib.Archive
{
    public class GameArchive : IDisposable
    {
        private readonly IGameArchiveReader _impl;

        public GameArchiveSpec Spec { get; }

        public string FilePath { get; }

        /// <exception cref="ArgumentNullException"><paramref name="filePath"/> or <paramref name="spec"/> is <see langword="null" />.</exception>
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
                    _impl = new GameDmgArchiveReader(new FileStream(filePath, FileMode.Open));
                    break;
                case OperatingSystem.Linux:
                    _impl = new GameTgzArchiveReader(new FileStream(filePath, FileMode.Open));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("spec");
            }
        }

        public IEnumerable<IArchiveEntry> Entries()
        {
            return _impl.Entries();
        }

        /// <summary>
        /// Extracts the contents of the archive to the specified directory.
        /// </summary>
        /// <param name="destinationDirectory">The folder to extract to. The directory will be created if it does not exist.</param>
        public async Task Extract(string destinationDirectory)
        {
            if (_impl is GameZipArchiveReader)
            {
                var impl = (GameZipArchiveReader)_impl;
                await impl.ExtractToDir(FilePath, destinationDirectory);
                return;
            }

            foreach (var entry in Entries())
            {
                var destinationPath = Path.Combine(destinationDirectory, entry.RelativePath);

                if (entry.IsFolder)
                {
                    Directory.CreateDirectory(destinationPath);
                }
                else
                {

                    // Assure that the directory exists, then create the file
                    var parentDirPath = Path.GetDirectoryName(destinationPath);
                    if (parentDirPath != null)
                    {
                        // If the parent dir is null, it means the file is located
                        // in the root folder, such as D:\foo.txt
                        Directory.CreateDirectory(parentDirPath);
                    }
                    using (var stream = File.Create(destinationPath))
                    {
                        entry.Extract(stream);
                    }

                }
            }
        }

        public void Dispose()
        {
            _impl.Dispose();
        }
    }
}
