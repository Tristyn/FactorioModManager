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

        // ReSharper disable once InconsistentNaming
        public OperatingSystem OperatingSystem { get; }

        public GameArchiveSpec(VersionNumber version, CpuArchitecture architecture, BuildConfiguration buildConfiguration, OperatingSystem operatingSystem)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            Version = version;
            Architecture = architecture;
            BuildConfiguration = buildConfiguration;
            OperatingSystem = operatingSystem;
        }

    }
}
