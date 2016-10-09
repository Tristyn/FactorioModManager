using System;
using FactorioModManager.Lib.Models;
using OperatingSystem = FactorioModManager.Lib.Models.OperatingSystem;
using Version = FactorioModManager.Lib.Models.Version;

namespace FactorioModManager.Lib.Web
{
    public class GameDownloadSpec
    {
        public Version Version { get; }

        public CpuArchitecture Architecture { get; }

        public InstallationType Type { get; }

        // ReSharper disable once InconsistentNaming
        public OperatingSystem OperatingSystem { get; }

        public GameDownloadSpec(Version version, CpuArchitecture architecture, InstallationType type, OperatingSystem operatingSystem)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            Version = version;
            Architecture = architecture;
            Type = type;
            OperatingSystem = operatingSystem;
        }

    }
}
