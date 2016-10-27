using System;
using System.IO;

namespace FactorioModManager.Lib.Files
{
    class IllegalPathCharactersException : IOException
    {
        public IllegalPathCharactersException() { }

        public IllegalPathCharactersException(string message)
            : base(message) { }

        public IllegalPathCharactersException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
