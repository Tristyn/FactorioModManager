using System;
using System.Collections.Generic;

namespace FactorioModManager.Lib.Models
{
    class ModPack
    {
        private Dictionary<string, Mod> _mods = new Dictionary<string, Mod>();
        private ModIOStrategy _ioStrategy;

        public ModPack(ModIOStrategy ioStrategy)
        {
            _ioStrategy = ioStrategy;
        }

        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
        public Mod Find(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Mod result;
            _mods.TryGetValue(name, out result);
            return result;
        }

        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
        public ModPack Remove(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Mod mod;
            if (!_mods.TryGetValue(name, out mod))
                return this;

            var updatedMods = new Dictionary<string, Mod>(_mods);
            updatedMods.Remove(name);
            throw new NotImplementedException();
            /*return new ModPack
            {
                _mods = updatedMods
            };*/
        }
        
        public ModPack Add(Mod mod)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the mod to this collection, it will replace an existing mod with the same name.
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public ModPack SetMod(Mod mod)
        {
            throw new NotImplementedException();
        }
    }
}
