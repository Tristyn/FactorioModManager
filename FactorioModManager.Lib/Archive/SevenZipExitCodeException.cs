namespace FactorioModManager.Lib.Archive
{
    class SevenZipExitCodeException : ArchiveException
    {
        public SevenZipExitCode ExitCode { get; }

        public SevenZipExitCodeException(SevenZipExitCode exitCode)
            : base(string.Format("SevenZip process exited with code {0}: {1}", exitCode.ToString("D"), exitCode.ToString("G")))
        {
            ExitCode = exitCode;
        }
    }
}
