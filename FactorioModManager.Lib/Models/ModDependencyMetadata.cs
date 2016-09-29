namespace FactorioModManager.Lib.Models
{
    public class ModDependencyMetadata
    {
        // ctor
        // validation

        public string ModName { get; internal set; }

        public string VersionName { get; internal set; }

        public Version UpperBound { get; internal set; }

        public VersionComparator UpperBoundComparator { get; internal set; }

        public Version LowerBound { get; internal set; }
        
        public VersionComparator LowerBoundComparator { get; internal set; }

        public enum VersionComparator : byte
        {
            EqualTo = 1,
            GreaterThan = 2,
            LessThan = 3,
            GreaterThanOrEqualTo = 4,
            LessThanOrEqualTo = 5
        }
    }
}
