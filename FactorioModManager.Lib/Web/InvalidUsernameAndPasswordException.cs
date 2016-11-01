using System;

namespace FactorioModManager.Lib.Web
{
    class InvalidUsernameAndPasswordException : ApplicationException
    {
        public InvalidUsernameAndPasswordException() { }

        public InvalidUsernameAndPasswordException(string message)
            : base(message) { }

        public InvalidUsernameAndPasswordException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
