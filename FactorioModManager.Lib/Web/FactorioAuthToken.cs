using System;
using System.Net;

namespace FactorioModManager.Lib.Web
{
    public class FactorioAuthToken
    {
        private readonly Cookie _sessionCookie;

        public FactorioAuthToken(Cookie sessionCookie)
        {
            if (sessionCookie == null)
                throw new ArgumentNullException("sessionCookie");

            _sessionCookie = sessionCookie;
        }

        public Cookie ToCookie()
        {
            return _sessionCookie;
        }
    }
}
