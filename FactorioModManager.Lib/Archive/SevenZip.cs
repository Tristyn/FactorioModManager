using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FactorioModManager.Lib.Files;
using FactorioModManager.Lib.Models;
using OperatingSystem = FactorioModManager.Lib.Models.OperatingSystem;

namespace FactorioModManager.Lib.Archive
{
    public static class SevenZip
    {
        /// <exception cref="ArgumentNullException"><paramref name="archiveFile"/> or <paramref name="outputDir"/> is <see langword="null" />.</exception>
        /// <exception cref="IllegalPathCharactersException">The archive or extraction path contained illegal characters.</exception>
        /// <exception cref="FailedToLaunchSevenZipException">The 7Zip executable could not be found or failed to launch.</exception>
        /// <exception cref="SevenZipExitCodeException">Seven zip returned with a bad exit code.</exception>
        public static async Task ExtractArchive(string archiveFile, string outputDir)
        {
            if (archiveFile == null)
                throw new ArgumentNullException("archiveFile");
            if (outputDir == null)
                throw new ArgumentNullException("outputDir");

            try
            {
                var archiveExtension = Path.GetExtension(archiveFile);
                // For more defensive code, check the extension for validity
                // For now this will throw an error code exception
                // https://sevenzip.osdn.jp/chm/general/formats.htm
            }
            catch (ArgumentException ex)
            {
                throw new IllegalPathCharactersException(ex.Message, ex);
            }

            /**
             * Uh oh it's arbitrary platform specific code in some random class.
             * YOU KNOW WHO I AM? YOU KNOW WHO I AM? I WORK FOR THE GOVERNMENT OK?
             * You know how many people work for the government in a day? 10,000
             * Do iou know how many die? 3,000
             * Do you know-
             * That's respect OK?
             */

            var sevenZipArgs =
                // x [archive]: extract specified archive
                "x \"" + archiveFile + "\" " +
                // -o [outputDir]
                "-o\"" + outputDir + "\" " +
                // Assume yes on all queries
                "-y " +
                // Overwrite All existing files without prompt.
                "-aoa " +
                // Recurse
                "-r";
                // Note: we could use the -bb1 flag and count the number of lines that start with "- " to return progress of the task. Need to enable piping standard error in the child process.
            var sevenZipExePath = GetExecutablePath();

            var sevenZipProcArgs = new ProcessStartInfo(sevenZipExePath, sevenZipArgs)
            {
                // The shell exists and direct StdIO but will be hidden from the user
                CreateNoWindow = true,
                // Don't use shell features like PATH, etc. Defaults to true
                UseShellExecute = false
            };


            try
            {
                using (var sevenZipProc = Process.Start(sevenZipProcArgs))
                {
                    // Process.Start never returns null when UseShellExecute = false
                    // ReSharper disable once PossibleNullReferenceException
                    while (!sevenZipProc.HasExited)
                    {
                        await Task.Delay(500);
                    }

                    var exitCode = (SevenZipExitCode) sevenZipProc.ExitCode;
                    switch (exitCode)
                    {
                        case SevenZipExitCode.Success:
                            break;
                        case SevenZipExitCode.Warning:
                            // This *should* get logged somewhere
                            break;
                        case SevenZipExitCode.FatalError:
                        case SevenZipExitCode.CommandLineError:
                        case SevenZipExitCode.OutOfMemoryError:
                        case SevenZipExitCode.ProcessEndedByUser:
                        default:
                            // Unknown code, eek!
                            throw new SevenZipExitCodeException(exitCode);
                    }
                }
            }

            catch (FileNotFoundException ex)
            {
                throw new FailedToLaunchSevenZipException("The 7Zip executable file could not be found. Ensure that the 7za file is in the directory used to probe for this applications assembly files.", ex);
            }

            catch (Win32Exception ex)
            {
                // An error occurred when parsing/loading the executable.

                throw new FailedToLaunchSevenZipException("An error occurred when opening the 7Zip executable.", ex);
            }

        }

        private static string GetExecutablePath()
        {
            var appBinDir = AppDomain.CurrentDomain.BaseDirectory;
            var isWindows = OperatingSystemEx.CurrentOS == OperatingSystem.Windows;

            return isWindows
                ? Path.Combine(appBinDir, "7z.exe")
                : Path.Combine(appBinDir, "7z ");
        }
    }
}
