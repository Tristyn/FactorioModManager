using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioModManager.Lib.Archive
{
    class FailedToLaunchSevenZipException : ArchiveException
    {
        public FailedToLaunchSevenZipException() { }

        public FailedToLaunchSevenZipException(string message)
            : base(message) { }

        public FailedToLaunchSevenZipException(string message, Exception innerException) 
            :base(message, innerException) { }
    }
}
