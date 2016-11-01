using System;

namespace FactorioModManager.Lib.Web
{
    /// <summary>
    /// Provides uris for mods.factorio.com.
    /// </summary>
    class FactorioModPortalUriFactory
    {
        private readonly Uri _modPortalUriBase;
        private readonly Uri _loginGatewayUri;
        private readonly Uri _newModPageUri;
        
        public FactorioModPortalUriFactory()
        {
            _modPortalUriBase = new Uri("https://www.factorio.com/");
            _loginGatewayUri = new Uri(_modPortalUriBase, "login");
            _newModPageUri = new Uri(_modPortalUriBase, "mods/new");
        }

        public Uri GetModPortalHomepageUri()
        {
            return _modPortalUriBase;
        }

        public Uri GetLoginGatewayUri()
        {
            return _loginGatewayUri;
        }

        public Uri GetNewModPageUri()
        {
            return _newModPageUri;
        }
    }
}
