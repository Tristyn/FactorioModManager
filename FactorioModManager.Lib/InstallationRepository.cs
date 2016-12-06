using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using FactorioModManager.Lib.Models;
using Nito.AsyncEx;

namespace FactorioModManager.Lib
{
    /// <summary>
    /// Provides access to the factorio updater and modpack management.
    /// </summary>
    public class InstallationRepository
    {
        private readonly Dictionary<InstallationSpec, Installation> _installations = new Dictionary<InstallationSpec, Installation>();   
        private readonly InstallationFactory _installationFactory;
        private readonly string _installationsDirectory;
        private readonly AsyncLock _lock = new AsyncLock();

        /// <exception cref="ArgumentNullException"><paramref name="storageDirectory"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="storageDirectory" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.-or-<paramref name="storageDirectory" /> is prefixed with, or contains, only a colon character (:).</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters. </exception>
        /// <exception cref="NotSupportedException"><paramref name="storageDirectory" /> contains a colon character (:) that is not part of a drive label ("C:\").</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="IOException">The directory specified by <paramref name="storageDirectory" /> is a file, it is on an unmapped drive, or the network name is not known.</exception>
        public static InstallationRepository Create(string storageDirectory)
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
            return new InstallationRepository(storageDirectory);
        }

        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="IOException">The directory specified by <paramref name="storageDirectory" /> is a file.-or-The network name is not known.</exception>
        private InstallationRepository(string storageDirectory)
        {
            _installationsDirectory = Path.Combine(storageDirectory);
            _installationFactory = new InstallationFactory(_installationsDirectory);
        }

        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="IOException">The storage directory specified by <paramref name="spec" /> is a file.-or-The network name is not known.</exception>
        public async Task<Installation> GetStandaloneInstallation(InstallationSpec spec)
        {
            using (await _lock.LockAsync())
            {

                Installation install;
                if (_installations.TryGetValue(spec, out install))
                    return install;

                install = await _installationFactory.CreateStandaloneInstallation(spec);
                _installations.Add(spec, install);
                return install;
            }
        }

        /// <exception cref="DirectoryNotFoundException"><paramref name="path" /> is invalid, such as referring to an unmapped drive. </exception>
        /// <exception cref="IOException"><paramref name="path" /> is a file name.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or combined exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.</exception>
        public IEnumerable<InstallationSpec> EnumerateInstallations()
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
                    // :( Could be some random folder created by the user that doesn't follow the naming convention.
                }
            }
            return installs;
        }
    }
}
