using System;
using System.IO;

namespace FactorioModManager.Lib.Files
{
    class InvalidPathException : IOException
    {
        public InvalidPathException() { }

        public InvalidPathException(string message)
            : base(message) { }

        public InvalidPathException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
