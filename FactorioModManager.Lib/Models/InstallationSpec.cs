using System;
using System.IO;
using CsQuery.EquationParser.Implementation;

namespace FactorioModManager.Lib.Models
{
    /// <summary>
    /// Describes the platform and version information of a Factorio installation.
    /// </summary>
    public class InstallationSpec : IEquatable<InstallationSpec>
    {
        public VersionNumber Version { get; }

        public CpuArchitecture Architecture { get; }

        public BuildConfiguration BuildConfiguration { get; }

        public string ExecutableRelativePath
        {
            get
            {
                switch (OperatingSystemEx.CurrentOS)
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

        public static bool operator ==(InstallationSpec a, InstallationSpec b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null)) return false;
            if (ReferenceEquals(b, null)) return false;

            return a.Version == b.Version
                && a.Architecture == b.Architecture
                && a.BuildConfiguration == b.BuildConfiguration;
        }

        public static bool operator !=(InstallationSpec a, InstallationSpec b)
        {
            if (ReferenceEquals(a, b)) return false;
            if (ReferenceEquals(a, null)) return true;
            if (ReferenceEquals(b, null)) return true;
            
            return a.Version != b.Version
                && a.Architecture != b.Architecture
                && a.BuildConfiguration != b.BuildConfiguration;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Version?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ (int) Architecture;
                hashCode = (hashCode*397) ^ (int) BuildConfiguration;
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((InstallationSpec) obj);
        }

        public bool Equals(InstallationSpec other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Version, other.Version) 
                && Architecture == other.Architecture 
                && BuildConfiguration == other.BuildConfiguration;
        }
    }
}
