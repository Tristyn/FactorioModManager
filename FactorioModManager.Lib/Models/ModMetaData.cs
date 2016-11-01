using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FactorioModManager.Lib.Models
{
    public sealed class ModMetadata
    {
        public string Name { get; internal set; }
        public VersionNumber Version { get; internal set; }
        public string Title { get; internal set; }
        public string Author { get; internal set; }
        public string Contact { get; internal set; }
        public string Homepage { get; internal set; }
        public string Description { get; internal set; }
        public IReadOnlyCollection<ModDependencyMetadata> Dependencies { get; internal set; } 

        private static readonly IReadOnlyCollection<ModDependencyMetadata>  EmptyDependencySingleton = new ReadOnlyCollection<ModDependencyMetadata>(new List<ModDependencyMetadata>());

        public ModMetadata()
        {
            Dependencies = EmptyDependencySingleton;
        }

        public ModMetadata(string name, string title, string author, VersionNumber version,
            string contact = null, string homepage = null, string description = null,
            IReadOnlyCollection<ModDependencyMetadata> dependencies = null)
        {
            if(dependencies != null && dependencies.Any(dep => dep == null))
                throw new ArgumentNullException("dependencies");

            Name = name;
            Title = title;
            Author = author;
            Version = version;
            Contact = contact;
            Homepage = homepage;
            Description = description;
            Dependencies = dependencies ?? EmptyDependencySingleton;
        }
        /*
        public JobResult IsValid()
        {
            var errors = new List<string>();
            const string nullError = "{0} can not be empty.";

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add(string.Format(nullError, "Name"));

            if (string.IsNullOrWhiteSpace(Author))
                errors.Add(string.Format(nullError, "Author"));

            if(string.IsNullOrWhiteSpace(Title))
                errors.Add(string.Format(nullError, "Title"));

            if (Version == null)
                errors.Add(string.Format(nullError, "Version"));

            return new JobResult(errors.ToArray());
        }
        */
    }
}
