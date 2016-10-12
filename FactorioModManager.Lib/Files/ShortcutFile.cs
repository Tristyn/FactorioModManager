using System;
using System.IO;
using IWshRuntimeLibrary;
using Mono.Unix;

namespace FactorioModManager.Lib.Files
{
    /// <summary>
    /// Represents a symbolic link or a shortcut on windows
    /// </summary>
    abstract class ShortcutFile
    {
        public string Path { get; }

        public static ShortcutFile New(string path)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    return new WindowsShortcut(path);
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    return new UnixSymLink(path);
                default:
                    throw new Exception("Unexpected OS :(");
            }
        }

        protected ShortcutFile(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            Path = path;
        }

        public abstract string GetTarget();

        public abstract void SetTarget(string path);

        public abstract bool Exists();

        public abstract bool TargetExists();

        private class UnixSymLink : ShortcutFile
        {
            private readonly UnixSymbolicLinkInfo _info;
            
            public UnixSymLink(string path)
                :base(path)
            {
                _info = new UnixSymbolicLinkInfo(path);
            }

            public override string GetTarget()
            {
                if (!Exists())
                    return null;

                using (var stream = new UnixFileInfo(Path).OpenRead())
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }

            public override void SetTarget(string targetPath)
            {
                if (targetPath == null)
                    throw new ArgumentNullException("targetPath");

                _info.CreateSymbolicLinkTo(targetPath);
            }

            public override bool Exists()
            {
                _info.Refresh();
                return _info.Exists;
            }

            public override bool TargetExists()
            {
                _info.Refresh();

                if (!_info.Exists)
                    return false;

                return new UnixFileInfo(GetTarget()).Exists;
            }
        }

        private class WindowsShortcut : ShortcutFile
        {
            // COM interop to the Windows Script Host Object Model (IWshRuntimeLibrary)
            // is the only easy way to create shortcuts sadly. :(

            private readonly FileInfo _info;
            private readonly IWshShortcut _shortcut;

            public WindowsShortcut(string path)
                : base(path + ".lnk")
            {
                _info = new FileInfo(Path);
                var wshShell = new WshShell();
                _shortcut = (IWshShortcut)wshShell.CreateShortcut(Path);
            }

            public override string GetTarget()
            {
                _shortcut.Load(Path);
                return _shortcut.TargetPath;
            }

            public override void SetTarget(string target)
            {
                _shortcut.TargetPath = target;
                _shortcut.Save();
            }

            public override bool Exists()
            {
                _info.Refresh();
                return _info.Exists;
            }

            public override bool TargetExists()
            {
                _shortcut.Load(Path);
                if (string.IsNullOrWhiteSpace(_shortcut.TargetPath))
                    return false;

                var targetFile = new FileInfo(_shortcut.TargetPath);
                return targetFile.Exists;
            }
        }
    }
}
