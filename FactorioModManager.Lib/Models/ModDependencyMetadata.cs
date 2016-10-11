using System;

namespace FactorioModManager.Lib.Models
{
    public sealed class ModDependencyMetadata
    {
        public string ModName { get; internal set; }
        
        public Version RequiredVersionUpperBound { get; internal set; }

        public VersionComparator RequiredVersionUpperBoundComparator { get; internal set; }

        public Version RequiredVersionLowerBound { get; internal set; }
        
        public VersionComparator RequiredVersionLowerBoundComparator { get; internal set; }

        public ModDependencyMetadata()
        {
            
        }

        public ModDependencyMetadata(
            string modName, 
            Version requiredVersionUpperBound = null, 
            VersionComparator requiredVersionUpperBoundComparator = VersionComparator.None, 
            Version requiredVersionLowerBound = null, VersionComparator 
            requiredVersionLowerBoundComparator = VersionComparator.None)
        {
            ModName = modName;
            RequiredVersionUpperBound = requiredVersionUpperBound;
            RequiredVersionUpperBoundComparator = requiredVersionUpperBoundComparator;
            RequiredVersionLowerBound = requiredVersionLowerBound;
            RequiredVersionLowerBoundComparator = requiredVersionLowerBoundComparator;
        }


        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(ModName))
                return false; // The mod name is required

            return true;
        }


        public enum VersionComparator : byte
        {
            None = 0,
            EqualTo = 1,
            GreaterThan = 2,
            LessThan = 3,
            GreaterThanOrEqualTo = 4,
            LessThanOrEqualTo = 5
        }
    }
}
