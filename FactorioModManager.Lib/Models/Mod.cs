using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FactorioModManager.Lib.Models
{
    public sealed class Mod
    {
        public ModMetadata Metadata { get; internal set; }

        public IReadOnlyCollection<Mod> Dependencies { get; internal set; }

        // private ModIOStrategy from dependency injection, static constructor with filepath etc

        public Mod()
        {

        }

        public Mod(ModMetadata metadata)
        {
            Metadata = metadata;
        }

        public Mod ConnectDependencies(IEnumerable<Mod> sisterMods)
        {
            var sisterDependencies = Dependencies.ToList();

            /*
             * Add a mod to dependencies if it can be found this mods metadata.
             * Validation is done at a later step,
             * don't worry about duplicates, wrong version requirements, etc here.
             */

            foreach (var sisterMod in sisterMods)
            {
                foreach (var dependency in Metadata.Dependencies)
                {
                    if (sisterMod.Matches(dependency))
                    {
                        sisterDependencies.Add(sisterMod);
                    }
                }
            }

            
            // Clone the mod with changes made
            return new Mod(Metadata)
            {
                Dependencies = new ReadOnlyCollection<Mod>(sisterDependencies)
            };
        }

        private bool Matches(ModDependencyMetadata modMetadata)
        {
            return modMetadata.ModName == Metadata.Name;
        }
    }
}
