using System;
using FactorioModManager.Lib.Models;
using OperatingSystem = FactorioModManager.Lib.Models.OperatingSystem;

namespace FactorioModManager.Lib.Web
{
    public class GameArchiveSpec
    {
        public VersionNumber Version { get; }

        public CpuArchitecture Architecture { get; }

        public BuildConfiguration BuildConfiguration { get; }
        
        public OperatingSystem OperatingSystem { get; }

        /// <exception cref="ArgumentNullException"><paramref name="version"/> is <see langword="null" />.</exception>
        public GameArchiveSpec(VersionNumber version, CpuArchitecture architecture, BuildConfiguration buildConfiguration, OperatingSystem operatingSystem)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            Version = version;
            Architecture = architecture;
            BuildConfiguration = buildConfiguration;
            OperatingSystem = operatingSystem;
        }

        /// <exception cref="ArgumentNullException"><paramref name="spec"/> is <see langword="null" />.</exception>
        public GameArchiveSpec(InstallationSpec spec, OperatingSystem os)
        {
            if (spec == null)
                throw new ArgumentNullException("spec");

            Version = spec.Version;
            Architecture = spec.Architecture;
            BuildConfiguration = spec.BuildConfiguration;
            OperatingSystem = os;
        }

        public string GetFileExtension()
        {
            return GetArchiveExtension(OperatingSystem);
        }

        /// <exception cref="ArgumentOutOfRangeException">os</exception>
        public static string GetArchiveExtension(OperatingSystem os)
        {
            switch (os)
            {
                case OperatingSystem.Windows:
                    return ".zip";
                case OperatingSystem.Mac:
                    return ".dmg";
                case OperatingSystem.Linux:
                    return ".tar.gz";
                default:
                    throw new ArgumentOutOfRangeException("os");
            }
        }
    }
}
