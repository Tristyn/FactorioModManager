using System;
using FactorioModManager.Lib.Models;
using Version = FactorioModManager.Lib.Models.Version;

namespace FactorioModManager.Lib.Web
{
    public class GameDownloadSpec
    {
        public Version Version { get; }

        public CpuArchitecture Architecture { get; }

        public InstallationType Type { get; }

        // ReSharper disable once InconsistentNaming
        public OS OS { get; }

        public GameDownloadSpec(Version version, CpuArchitecture architecture, InstallationType type, OS os)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            Version = version;
            Architecture = architecture;
            Type = type;
            OS = os;
        }

    }
}
