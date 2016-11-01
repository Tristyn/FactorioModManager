using System;

namespace FactorioModManager.Lib.Models
{
    class ModDownloadSpec
    {
        public string Name { get; }
        public VersionNumber Version { get; }

        /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="version"/> is <see langword="null" />.</exception>
        public ModDownloadSpec(string name, VersionNumber version)
        {
            Name = name;
            Version = version;

            if (name == null)
                throw new ArgumentNullException("name");
            if (version == null)
                throw new ArgumentNullException("version");
        }

        public override string ToString()
        {
            return string.Format("{0}_{1}", Name, Version.ToString("."));
        }
    }
}
