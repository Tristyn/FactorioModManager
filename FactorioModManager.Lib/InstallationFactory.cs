using System;
using System.IO;
using FactorioModManager.Lib.Contracts;
using FactorioModManager.Lib.Models;

namespace FactorioModManager.Lib
{
    internal class InstallationFactory
    {
        private readonly string _standaloneInstallationsDirectory;

        /// <exception cref="ArgumentNullException"><paramref name="standaloneInstallationsDirectory"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">The path contained invalid characters.</exception>
        /// <exception cref="IOException">The directory specified by <paramref name="path" /> is a file.-or-The network name is not known.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters. </exception>
        /// <exception cref="NotSupportedException"><paramref name="path" /> contains a colon character (:) that is not part of a drive label ("C:\").</exception>
        public InstallationFactory(string standaloneInstallationsDirectory)
        {
            if (standaloneInstallationsDirectory == null)
                throw new ArgumentNullException("standaloneInstallationsDirectory");

            if(standaloneInstallationsDirectory.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                throw new ArgumentException("The path contained invalid characters.", "standaloneInstallationsDirectory");

            Directory.CreateDirectory(standaloneInstallationsDirectory);

            _standaloneInstallationsDirectory = standaloneInstallationsDirectory;
        }

        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="IOException">The storage directory specified by <paramref name="spec" /> is a file.-or-The network name is not known.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path" /> is null. </exception>
        /// <exception cref="ArgumentException"><paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.-or-<paramref name="path" /> is prefixed with, or contains, only a colon character (:).</exception>
        /// <exception cref="NotSupportedException"><paramref name="path" /> contains a colon character (:) that is not part of a drive label ("C:\").</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters. </exception>
        public Installation CreateStandaloneInstallation(InstallationSpec spec)
        {
            var gamePath = Path.Combine(_standaloneInstallationsDirectory, spec.ToString());

            Directory.CreateDirectory(gamePath);

            return new Installation(spec, gamePath);
        }
    }
}
