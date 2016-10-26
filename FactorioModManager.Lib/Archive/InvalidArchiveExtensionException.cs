using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioModManager.Lib.Archive
{
    class InvalidArchiveExtensionException : ArchiveException
    {
        public InvalidArchiveExtensionException() { }

        public InvalidArchiveExtensionException(string message) : base(message)
        {
        }

        public InvalidArchiveExtensionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
