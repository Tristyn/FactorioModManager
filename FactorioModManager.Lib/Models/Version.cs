using System;

namespace FactorioModManager.Lib.Models
{
    public sealed class Version
    {
        public Version()
        {

        }

        public Version(long major, long minor, long revision)
        {
            MajorVersion = major;
            MinorVersion = minor;
            Revision = revision;
        }

        public long MajorVersion { get; }
        public long MinorVersion { get; }
        public long Revision { get; }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", MajorVersion, MinorVersion, Revision);
        }

        private bool Equals(Version other)
        {
            return MajorVersion == other.MajorVersion
                && MinorVersion == other.MinorVersion
                && Revision == other.Revision;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Version && Equals((Version) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MajorVersion.GetHashCode();
                hashCode = (hashCode*397) ^ MinorVersion.GetHashCode();
                hashCode = (hashCode*397) ^ Revision.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Version a, Version b)
        {
            if ((object)a == null)
            {
                return (object)b == null;
            }
            if ((object)b == null)
                return false;

            return a.MajorVersion == b.MajorVersion
                   && a.MinorVersion == b.MinorVersion
                   && a.Revision == b.Revision;
        }

        public static bool operator !=(Version a, Version b)
        {
            if ((object) a == null)
            {
                return (object) b != null;
            }
            if ((object) b == null)
                return true;

            return a.MajorVersion != b.MajorVersion
                   || a.MinorVersion != b.MinorVersion
                   || a.Revision != b.Revision;
        }

        public static bool operator >(Version a, Version b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            return a.MajorVersion > b.MajorVersion
                   || a.MinorVersion > b.MinorVersion
                   || a.Revision > b.Revision;
        }

        public static bool operator <(Version a, Version b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            return a.MajorVersion < b.MajorVersion
                   || a.MinorVersion < b.MinorVersion
                   || a.Revision < b.Revision;
        }

        public static bool operator >=(Version a, Version b)
        {
            // Note: returns true when a and b are null

            if (a == b)
                return true;

            return a > b;
        }

        public static bool operator <=(Version a, Version b)
        {
            // Note: returns true when a and b are null

            if (a == b)
                return true;

            return a < b;
        }
    }
}
