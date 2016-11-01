using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace FactorioModManager.Lib.Web
{
    public class FactorioUserSessionToken
    {
        // FactorioUserSessionToken is meant to be immutable.
        // CookieCollections are cloned throughout to defend against 
        // side effects and mutations of calling code.

        private readonly CookieCollection _sessionCookies;

        /// <exception cref="ArgumentNullException"><paramref name="modPortalCookies"/> or <paramref name="homePageCookies"/> is <see langword="null" />.</exception>
        public FactorioUserSessionToken(CookieCollection modPortalCookies, CookieCollection homePageCookies)
        {
            if (modPortalCookies == null)
                throw new ArgumentNullException("modPortalCookies");
            if (homePageCookies == null)
                throw new ArgumentNullException("homePageCookies");

            var aggregate = new CookieCollection
            {
                modPortalCookies,
                homePageCookies
            };

            _sessionCookies = CopyCollection(aggregate);
        }

        public CookieCollection ToCookieCollection()
        {
            return CopyCollection(_sessionCookies);
        }

        private CookieCollection CopyCollection(CookieCollection container)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, container);
                stream.Seek(0, SeekOrigin.Begin);
                return (CookieCollection)formatter.Deserialize(stream);
            }
        }
    }
}
