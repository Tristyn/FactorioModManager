using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using NodaTime;

namespace FactorioModManager.Lib.Web
{
    /// <summary>
    /// Provides management and renewal services for a factorio web user session.
    /// </summary>
    public class FactorioUserSession
    {
        private static readonly Duration SessionTokenValidDuration = Duration.FromDays(13);

        private readonly AnonymousFactorioWebClient _client;
        private readonly AsyncLock _lock = new AsyncLock();
        private readonly Func<Tuple<
            IObservable<FactorioUserCredentials>,
            IObserver<FactorioAuthResult>>> _challengeResponseFactory;
        
        private FactorioUserSessionToken _sessionToken;
        private Instant _sessionTokenExpiryDate;

        public FactorioUserSession(AnonymousFactorioWebClient client, Func<Tuple<IObservable<FactorioUserCredentials>, IObserver<FactorioAuthResult>>> challengeResponseFactory)
        {
            _client = client;
            _challengeResponseFactory = challengeResponseFactory;
        }

        /// <summary>
        /// Discards the user session.
        /// Note that this does not log the user out of any web services.
        /// </summary>
        public async Task Invalidate()
        {
            using (await _lock.LockAsync())
            {
                _sessionToken = null;
            }
        }

        // ReSharper disable ExceptionNotThrown
        /// <exception cref="WebException">Request failed due to a bad connection (e.g. no internet).</exception>
        /// <exception cref="HttpRequestException">Recieved an error status code that could not be recovered from.</exception>
        /// <exception cref="InvalidUsernameAndPasswordException"></exception>
        // ReSharper restore ExceptionNotThrown
        public async Task<CookieCollection> GetSessionCookies()
        {
            using (await _lock.LockAsync())
            {
                if (_sessionTokenExpiryDate > SystemClock.Instance.GetCurrentInstant() 
                    || _sessionToken != null)
                    return _sessionToken.ToCookieCollection();

                var credentialsSource = _challengeResponseFactory();

                Exception lastAuthException = null;
                var credentialsStream = credentialsSource.Item1;
                var challengeResponsesStream = credentialsSource.Item2;
                challengeResponsesStream.OnNext(FactorioAuthResult.None);
                await credentialsStream.ForEachAsync(async credentials =>
                {
                    // This is an async void method, so any unhandled exceptions
                    // will be unhandled in the thread and crash the application.
                    // Catch and store them in the closure variable forEachException
                    // and handle it there.
                    // The last exception will bubble up, as it represents the users last attempt
                    // to auth before canceling.
                    try
                    {
                        if (credentials == null)
                        {
                            challengeResponsesStream.OnError(new ArgumentNullException());
                        }

                        _sessionToken = await _client.Authorize(credentials);

                        // Clear the exception because we authed successfully
                        lastAuthException = null;
                        _sessionTokenExpiryDate = SystemClock.Instance.GetCurrentInstant() + SessionTokenValidDuration;
                        challengeResponsesStream.OnNext(FactorioAuthResult.Success);
                    }
                    catch (InvalidUsernameAndPasswordException ex)
                    {
                        lastAuthException = ex;
                        challengeResponsesStream.OnError(ex);
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine(ex.Message);
                        lastAuthException = ex;
                        challengeResponsesStream.OnError(ex);
                    }
                    catch (WebException ex)
                    {
                        Console.WriteLine(ex.Message);
                        lastAuthException = ex;
                        challengeResponsesStream.OnError(ex);
                    }
                    catch (Exception ex)
                    {
                        challengeResponsesStream.OnError(ex);
                        lastAuthException = ex;
                    }
                });

                if (lastAuthException != null)
                    throw lastAuthException;

                return _sessionToken.ToCookieCollection();
            }
        }

        public enum FactorioAuthResult
        {
            None,
            Success
        }
    }
}
