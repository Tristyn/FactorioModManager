using System;
using System.IO;

namespace FactorioModManager.Lib.Models
{
    /// <summary>
    /// Describes the platform and version information of a Factorio installation.
    /// </summary>
    public class InstallationSpec
    {
        public VersionNumber Version { get; }

        public CpuArchitecture Architecture { get; }

        public BuildConfiguration BuildConfiguration { get; }

        public string ExecutableRelativePath
        {
            get
            {
                switch (OperatingSystemEx.CurrentOSVersion)
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

        /// <exception cref="ArgumentNullException"><paramref name="version"/> is <see langword="null" />.</exception>
        public InstallationSpec(VersionNumber version, CpuArchitecture architecture, BuildConfiguration buildConfiguration)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            Version = version;
            Architecture = architecture;
            BuildConfiguration = buildConfiguration;
        }

        public override string ToString()
        {
            return string.Format("{0}_{1}_{2}", Version, BuildConfiguration, Architecture);
        }

        /// <exception cref="ArgumentNullException"><paramref name="specString"/> is <see langword="null" />.</exception>
        /// <exception cref="FormatException"></exception>
        public static InstallationSpec Parse(string specString)
        {
            if (specString == null)
                throw new ArgumentNullException("specString");

            var parts = specString.Split('_');
            if (parts.Length != 3)
                throw new FormatException();

            var version = VersionNumber.Parse(parts[0]);

            BuildConfiguration build;
            if (!Enum.TryParse(parts[1], out build))
                throw new FormatException();

            CpuArchitecture architecture;
            if (!Enum.TryParse(parts[2], out architecture))
                throw new FormatException();

            return new InstallationSpec(version, architecture, build);
        }
    }
}
