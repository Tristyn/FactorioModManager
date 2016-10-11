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

        public GameArchive(Stream readStream, GameArchiveSpec spec)
        {
            if (readStream == null)
                throw new ArgumentNullException("readStream");
            if (spec == null)
                throw new ArgumentNullException("spec");

            Spec = spec;

            switch (spec.OperatingSystem)
            {
                case OperatingSystem.Windows:
                    _impl = new GameZipArchiveReader(readStream);
                    break;
                case OperatingSystem.Mac:
                    _impl = new GameDmgArchiveReader(readStream);
                    break;
                case OperatingSystem.Linux:
                    _impl = new GameTgzArchiveReader(readStream);
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
        public Task Extract(string destinationDirectory)
        {
            return Task.Run(() =>
            {
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
            });
        }

        public void Dispose()
        {
            _impl.Dispose();
        }
    }
}
