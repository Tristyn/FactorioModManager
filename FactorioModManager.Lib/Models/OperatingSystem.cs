using System;

namespace FactorioModManager.Lib.Models
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// The supported operating systems for Factorio.
    /// </summary>
    public enum OperatingSystem
    {
        Windows, Mac, Linux//, Android (COME ON KOVAREX PLEASE JUST SUPPORT IT ALREADY)
    }

    public static class OperatingSystemEx
    {
        public static OperatingSystem CurrentOS => ToFactorioSupportedOperatingSystem(Environment.OSVersion.Platform);


        public static OperatingSystem ToFactorioSupportedOperatingSystem(this PlatformID platformId)
        {
            switch (platformId)
            {
                case PlatformID.Win32NT:
                    return OperatingSystem.Windows;
                case PlatformID.Unix:
                    return OperatingSystem.Linux;
                case PlatformID.MacOSX:
                    return OperatingSystem.Mac;
                default:
                    throw new ArgumentOutOfRangeException("platformId");
            }
        }
    }
}
