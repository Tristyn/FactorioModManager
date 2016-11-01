using System.IO;
using System.Threading.Tasks;

namespace FactorioModManager.Lib.Models
{
    abstract class ModIOStrategy
    {
        public abstract Task<bool> Exists();

        public abstract Task CopyTo(string path);

        public abstract Task<ModMetadata> GetMetadata();

        public abstract Task<Stream> ToStream();
    }
}
