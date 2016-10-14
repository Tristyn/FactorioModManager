using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using FactorioModManager.Lib.Models;

namespace FactorioModManager.Lib
{
    /// <summary>
    /// Provides access to the factorio updater and modpack management.
    /// </summary>
    public class StandaloneInstallationManager
    {
        private readonly InstallationFactory _installationFactory;
        private readonly string _installationsDirectory;

        /// <exception cref="ArgumentNullException"><paramref name="storageDirectory"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="storageDirectory" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.-or-<paramref name="path" /> is prefixed with, or contains, only a colon character (:).</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters. </exception>
        /// <exception cref="NotSupportedException"><paramref name="storageDirectory" /> contains a colon character (:) that is not part of a drive label ("C:\").</exception>
        /// <exception cref="IOException">The directory specified by <paramref name="storageDirectory" /> is a file, it is on an unmapped drive, or the network name is not known.</exception>
        public static StandaloneInstallationManager Create(string storageDirectory)
        {
            if (storageDirectory == null)
                throw new ArgumentNullException("storageDirectory");
            try
            {
                Directory.CreateDirectory(storageDirectory);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new IOException(ex.Message, ex);
            }
            return new StandaloneInstallationManager(storageDirectory);
        }

        private StandaloneInstallationManager(string storageDirectory)
        {
            _installationsDirectory = Path.Combine(storageDirectory);
            _installationFactory = new InstallationFactory(_installationsDirectory);
        }

        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="IOException">The storage directory specified by <paramref name="spec" /> is a file.-or-The network name is not known.</exception>
        public Installation GetStandaloneInstallation(InstallationSpec spec)
        {
            return _installationFactory.CreateStandaloneInstallation(spec);
        }

        /// <exception cref="DirectoryNotFoundException"><paramref name="path" /> is invalid, such as referring to an unmapped drive. </exception>
        /// <exception cref="IOException"><paramref name="path" /> is a file name.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or combined exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.</exception>
        public IEnumerable<InstallationSpec> GetInstallations()
        {
            var installs = new List<InstallationSpec>();
            foreach (var dir in Directory.EnumerateDirectories(_installationsDirectory))
            {
                var dirName = Path.GetFileName(dir);
                try
                {
                    installs.Add(InstallationSpec.Parse(dirName));
                }
                catch (FormatException)
                {
                    // :( Could be some random folder created by the user that doesn't follow convention.
                }
            }
            return installs;
        }
    }
}
