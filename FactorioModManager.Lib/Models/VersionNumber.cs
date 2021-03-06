﻿using System;
using System.Linq;

namespace FactorioModManager.Lib.Models
{
    public sealed class VersionNumber : IComparable<VersionNumber>
    {
        public VersionNumber()
        {

        }

        public VersionNumber(long major, long minor, long revision)
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
            return ToString(".");
        }

        /// <exception cref="ArgumentNullException"><paramref name="delimiter"/> is <see langword="null" />.</exception>
        public string ToString(string delimiter)
        {
            if (delimiter == null)
                throw new ArgumentNullException("delimiter");

            return MajorVersion + delimiter + MinorVersion + delimiter + Revision;
        }

        private bool Equals(VersionNumber other)
        {
            return MajorVersion == other.MajorVersion
                && MinorVersion == other.MinorVersion
                && Revision == other.Revision;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is VersionNumber && Equals((VersionNumber) obj);
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

        /// <exception cref="FormatException">Not a valid version string.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="versionString"/> is <see langword="null" />.</exception>
        public static VersionNumber Parse(string versionString)
        {
            if (versionString == null)
                throw new ArgumentNullException("versionString");

            var ints = versionString
                .Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            if(ints.Count != 3)
                throw new FormatException("Not a valid version string.");

            return new VersionNumber(ints[0], ints[1], ints[2]);
        }

        public static bool operator ==(VersionNumber a, VersionNumber b)
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

        public static bool operator !=(VersionNumber a, VersionNumber b)
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

        public static bool operator >(VersionNumber a, VersionNumber b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            return a.MajorVersion > b.MajorVersion
                   || a.MinorVersion > b.MinorVersion
                   || a.Revision > b.Revision;
        }

        public static bool operator <(VersionNumber a, VersionNumber b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            return a.MajorVersion < b.MajorVersion
                   || a.MinorVersion < b.MinorVersion
                   || a.Revision < b.Revision;
        }

        public static bool operator >=(VersionNumber a, VersionNumber b)
        {
            // Note: returns true when a and b are null

            if (a == b)
                return true;

            return a > b;
        }

        public static bool operator <=(VersionNumber a, VersionNumber b)
        {
            // Note: returns true when a and b are null

            if (a == b)
                return true;

            return a < b;
        }

        public int CompareTo(VersionNumber other)
        {
            if (MajorVersion > other.MajorVersion)
                return 1;
            if (MajorVersion < other.MajorVersion)
                return -1;

            if (MinorVersion > other.MinorVersion)
                return 1;
            if (MinorVersion < other.MinorVersion)
                return -1;

            if (Revision > other.Revision)
                return 1;
            if (Revision < other.Revision)
                return -1;

            return 0;
        }
    }
}
