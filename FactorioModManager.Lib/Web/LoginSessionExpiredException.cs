using System;

namespace FactorioModManager.Lib.Web
{
    /// <summary>
    /// The session has expired and could be renewed by re-authing.
    /// </summary>
    class LoginSessionExpiredException : ApplicationException
    {
        public LoginSessionExpiredException() { }

        public LoginSessionExpiredException(string message)
            : base(message) { }

        public LoginSessionExpiredException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
