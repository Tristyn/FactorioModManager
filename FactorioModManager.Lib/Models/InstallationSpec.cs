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
        
        public BuildConfiguration Type { get; }
        
        public string ExecutableRelativePath
        {
            get
            {
                switch (Environment.OSVersion.Platform.ToFactorioSupportedOperatingSystem())
                {
                    case OperatingSystem.Windows:
                        return Path.Combine("bin", "factorio.exe");
                    case OperatingSystem.Mac:
                        return Path.Combine("factorio.app", "Contents", "macOS", "factorio");
                    case OperatingSystem.Linux:
                        return Path.Combine("bin", "factorio");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public InstallationSpec(Version version, CpuArchitecture architecture, BuildConfiguration type)
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
