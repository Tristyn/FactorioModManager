using System;

namespace FactorioModManager.Lib.Archive
{
    class InvalidGameArchiveContentsException : ArchiveException
    {
        public InvalidGameArchiveContentsException() { }

        public InvalidGameArchiveContentsException(string message)
            : base(message) { }

        public InvalidGameArchiveContentsException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
