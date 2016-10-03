using FactorioModManager.Lib.Contracts;

namespace FactorioModManager.Lib
{
    public class FactorioWebClient : IModPortalClient
    {
        /*
Game downloading notes:
factorio.com/get-download/{version}/{build}/{platform}

build: alpha, demo, headless (only linux64 platform)

platform: win64: .exe, win64-manual: .zip, win32, .exe win32-manual: .zip, osx: .dmg, linux64: .tar.gz, linux32: .tar.gz,



factorio package feeds
/download: stable release
/download/experimental: experimental release

query body > div.container > h3 to scrape version numbers
         */


    }
}
