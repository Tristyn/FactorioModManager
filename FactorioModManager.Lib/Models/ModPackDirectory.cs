using System;

namespace FactorioModManager.Lib.Models
{
    public class ModPackDirectory
    {
        public string Directory { get; }

        public ModPackDirectory(string directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            Directory = directory;
        }
    }
}
