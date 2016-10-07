using System;
using System.IO;

namespace FactorioModManager.Lib.Models
{
    /// <summary>
    /// Describes the platform and version information of a Factorio installation.
    /// </summary>
    public class InstallationSpec
    {
        public Version Version { get; }

        public CpuArchitecture Architecture { get; }
        
        public InstallationType Type { get; }
        
        public string ExecutableRelativePath => Path.Combine("bin", "factorio.exe");

        public InstallationSpec(Version version, CpuArchitecture architecture, InstallationType type)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            Version = version;
            Architecture = architecture;
            Type = type;
        }

        public override string ToString()
        {
            return string.Format("{0}_{1}_{2}", Version, Type, Architecture);
        }
    }
}
