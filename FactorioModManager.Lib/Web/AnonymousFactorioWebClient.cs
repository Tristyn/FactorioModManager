using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CsQuery;

namespace FactorioModManager.Lib.Web
{
    public class AnonymousFactorioWebClient : IAnonymousFactorioWebClient
    {
        private readonly HttpClient _anonymousClient = new HttpClient();
        private readonly FactorioHomepageUriFactory _homepageUriFactory
            = new FactorioHomepageUriFactory();
        private readonly FactorioModPortalUriFactory _modPortalUriFactory
            = new FactorioModPortalUriFactory();

        /// <exception cref="InvalidUsernameAndPasswordException">The server rejected the login credentials.</exception>
        /// <exception cref="HttpRequestException">Recieved an error status code that could not be recovered from.</exception>
        /// <exception cref="WebException">Request failed due to a bad connection (e.g. no internet).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="creds"/> is <see langword="null" />.</exception>
        public async Task<FactorioUserSessionToken> Authorize(FactorioUserCredentials creds)
        {
            if (creds == null)
                throw new ArgumentNullException("creds");

            var modPortalSessionCookies = await AuthorizeModPortal(creds.UsernameOrEmail, creds.Password);
            var homepageSession = await AuthorizeHomepage(creds.UsernameOrEmail, creds.Password);
            
            return new FactorioUserSessionToken(modPortalSessionCookies, new CookieCollection { homepageSession });
        }

        // ReSharper disable ExceptionNotThrown
        /// <exception cref="WebException">Request failed due to a bad connection (e.g. no internet).</exception>
        /// <exception cref="InvalidUsernameAndPasswordException">The server rejected the login credentials.</exception>
        /// <exception cref="HttpRequestException">Recieved an error status code that could not be recovered from.</exception>
        // ReSharper restore ExceptionNotThrown
        private async Task<Cookie> AuthorizeHomepage(string usernameOrEmail, string password)
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

            CookieContainer cookies;
            var loginClient = BuildHttpClientWithCookies(out cookies);

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
            response.EnsureSuccessStatusCode();
            if (response.StatusCode != HttpStatusCode.Redirect ||
                response.Headers.Location != _homepageUriFactory.GetHomepage())
                throw new InvalidUsernameAndPasswordException();

            var cookieDomain = _homepageUriFactory.GetHomepage();
            return cookies.GetCookies(cookieDomain)["session"];
        }

        /// <exception cref="WebException">Request failed due to a bad connection (e.g. no internet).</exception>
        /// <exception cref="InvalidUsernameAndPasswordException">The server rejected the login credentials.</exception>
        /// <exception cref="HttpRequestException">Recieved an error status code that could not be recovered from.</exception>
        private async Task<CookieCollection> AuthorizeModPortal(string usernameOrEmail, string password)
        {
            var loginUri = _modPortalUriFactory.GetLoginGatewayUri();
            var container = await LoginViaAuthPortalGateway(loginUri, usernameOrEmail, password);

            var modPortalRoot = _modPortalUriFactory.GetModPortalHomepageUri();
            return container.GetCookies(modPortalRoot);
        }

        // ReSharper disable ExceptionNotThrown
        /// <exception cref="HttpRequestException">Recieved an error status code that could not be recovered from.</exception>
        /// <exception cref="WebException">Request failed due to a bad connection (e.g. no internet).</exception>
        /// <exception cref="InvalidUsernameAndPasswordException">The server rejected the login credentials.</exception>
        private async Task<CookieContainer> LoginViaAuthPortalGateway(Uri loginGatewayUri, string usernameOrEmail, string password)
        {
            var cookies = new CookieContainer();
            var client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = cookies
            });

            // Retrieve the login page by using the gateway
            var loginPageResponse = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, loginGatewayUri));
            loginPageResponse.EnsureSuccessStatusCode();
            var authPage = await loginPageResponse.Content.ReadAsStringAsync();
            var authPageDom = CQ.CreateDocument(authPage);
            var form = authPageDom.Select(".login form").First();

            // Prepare to send the form found in the login page
            // Select the hidden inputs so they can be included in the actual login POST request.
            var hiddenInputs = form
                .Select("input[type=hidden]")
                .Select(element => new KeyValuePair<string, string>(
                    element.GetAttribute("name"),
                    element.GetAttribute("value")));

            var formFields = hiddenInputs.Concat(new[]
            {
                new KeyValuePair<string, string>("username", usernameOrEmail),
                new KeyValuePair<string, string>("password", password),
            });
            var formRequest = new FormUrlEncodedContent(formFields);
            var formAction = new Uri(form.Attr("action"));
            var formMethod = new HttpMethod(form.Attr("method"));
            var sendFormMsg = new HttpRequestMessage(formMethod, formAction)
            {
                Content = formRequest
            };
            var response = await client.SendAsync(sendFormMsg);

            /**
             * The /login/process handler is a little odd in that both auth
             * success and auth failure have a 200 OK status code.
             * We determine if authorizing worked by checking if we were
             * redirected to the same login form.
             */
            response.EnsureSuccessStatusCode();
            // Did we recieve the same login form response again?
            if (response.RequestMessage.RequestUri.Host == loginPageResponse.RequestMessage.RequestUri.AbsolutePath)
            {
                // Login failed and the server resent the login form with an error message.
                throw new InvalidUsernameAndPasswordException("The server rejected the login credentials.");
            }

            return cookies;
        }

        // ReSharper disable ExceptionNotThrown
        /// <exception cref="WebException">Request failed due to a bad connection (e.g. no internet).</exception>
        /// <exception cref="HttpRequestException">Recieved an error status code that could not be recovered from.</exception>
        // ReSharper restore ExceptionNotThrown
        private async Task<string> GetHomepageLoginCsrfToken()
        {
            var uri = _homepageUriFactory.GetLoginPage();
            var loginHtml = await _anonymousClient.GetStringAsync(uri);
            var loginDom = CQ.CreateDocument(loginHtml);

            // There is only one form, so grab the token directly
            var csrfElement = loginDom["input[name=csrf_token]"].Single();
            return csrfElement.Value;
        }

        private HttpClient BuildHttpClientWithCookies(out CookieContainer cookies)
        {
            return new HttpClient(new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = cookies = new CookieContainer()
            });
        }

        public void Dispose()
        {
            _anonymousClient.Dispose();
        }
    }
}
