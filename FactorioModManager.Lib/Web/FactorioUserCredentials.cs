using System;

namespace FactorioModManager.Lib.Web
{
    public class FactorioUserCredentials
    {
        /// <exception cref="ArgumentNullException"><paramref name="usernameOrEmail"/> or <paramref name="password"/> is <see langword="null" />.</exception>
        public FactorioUserCredentials(string usernameOrEmail, string password)
        {
            if (usernameOrEmail == null)
                throw new ArgumentNullException("usernameOrEmail");
            if (password == null)
                throw new ArgumentNullException("password");

            UsernameOrEmail = usernameOrEmail;
            Password = password;
        }

        public string UsernameOrEmail { get; }
        public string Password { get; }
    }
}
