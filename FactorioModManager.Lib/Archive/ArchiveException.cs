using System;

namespace FactorioModManager.Lib.Archive
{
    class ArchiveException : ApplicationException
    {
        public ArchiveException() { }

        public ArchiveException(string message)
            : base(message) { }

        public ArchiveException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
