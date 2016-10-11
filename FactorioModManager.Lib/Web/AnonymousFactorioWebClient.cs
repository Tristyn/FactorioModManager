using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CsQuery;

namespace FactorioModManager.Lib.Web
{
    public class AnonymousFactorioWebClient : IAnonymousFactorioWebClient
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly FactorioHomepageUriFactory _homepageUriFactory
            = new FactorioHomepageUriFactory();

        public async Task<FactorioAuthToken> AuthorizeHomepage(string usernameOrEmail, string password)
        {
            /* 
             * url: factorio.com/login
             * method: post
             * request content type: application/x-www-form-urlencoded
             * 
             *  == form fields: ==
             * csrf_token: string scraped from get: factorio.com/login form
             * username_or_email: string
             * password: string
             * action: string = "Login"
             */
             
            var cookies = new CookieContainer();
            var loginClient = new HttpClient(new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = cookies
            });

            var uri = _homepageUriFactory.GetLoginPage();
            var csrfToken = await GetHomepageLoginCsrfToken();

            var args = new[]
            {
                new KeyValuePair<string,string>("csrf_token", csrfToken),
                new KeyValuePair<string,string>("username_or_email", usernameOrEmail),
                new KeyValuePair<string,string>("password", password),
                new KeyValuePair<string,string>("action", "Login")
            };

            var response = await loginClient.PostAsync(uri, new FormUrlEncodedContent(args));
            if (response.StatusCode != HttpStatusCode.Redirect ||
                response.Headers.Location != _homepageUriFactory.GetHomepage()) return null;

            var cookieDomain = _homepageUriFactory.GetHomepage();
            var sessionCookie = cookies.GetCookies(cookieDomain)["session"];
            return new FactorioAuthToken(sessionCookie);
        }

        private async Task<string> GetHomepageLoginCsrfToken()
        {
            var uri = _homepageUriFactory.GetLoginPage();
            var loginHtml = await _client.GetStringAsync(uri);
            var loginDom = CQ.CreateDocument(loginHtml);

            // There is only one form, so grab the token directly
            var csrfElement = loginDom["input[name=csrf_token]"].Single();
            return csrfElement.Value;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
