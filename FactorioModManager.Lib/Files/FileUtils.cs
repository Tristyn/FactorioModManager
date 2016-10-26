using System;
using System.IO;

namespace FactorioModManager.Lib.Files
{
    public static class FileUtils
    {
        public static string GetTempDir()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }


        /// <summary>
        /// Moves a directory with a fast path for dirs within the same volume.
        /// The target dir must be empty or not exist.
        /// </summary>
        /// <exception cref="IOException">The directory specified by <paramref name="sourceDir" /> is a file.-or-The network name is not known.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters. </exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        public static void MoveDirectoryAcrossVolume(string sourceDir, string targetDir)
        {
            try
            {
                Directory.Delete(targetDir, recursive: true);
                Directory.Move(sourceDir, targetDir);
            }
            catch (IOException ex)
            {
                Directory.CreateDirectory(targetDir);

                foreach (var file in Directory.GetFiles(sourceDir))
                {
                    if (file == null)
                        continue;

                    try
                    {
                        File.Move(file, Path.Combine(targetDir, Path.GetFileName(file)));
                    }
                    catch (FileNotFoundException)
                    {
                        // Lol we can skip it
                    }
                }
                foreach (var directory in Directory.GetDirectories(sourceDir))
                    MoveDirectoryAcrossVolume(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
            }
        }
    }
}
