namespace FactorioModManager.Lib.Archive
{
    enum SevenZipExitCode
    {
        Success = 0,
        Warning = 1,
        FatalError = 2,
        CommandLineError = 7,
        OutOfMemoryError = 8,
        ProcessEndedByUser = 255
    }
}
