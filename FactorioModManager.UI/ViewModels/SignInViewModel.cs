using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FactorioModManager.Lib.Web;
using FactorioModManager.UI.Extensions;
using FactorioModManager.UI.Framework;
using ReactiveUI;

namespace FactorioModManager.UI.ViewModels
{
    public class SignInViewModel : ViewModelBase
    {
        private readonly AnonymousFactorioWebClient _webClient;

        private string _usernameOrEmail;
        private string _password;

        /// <exception cref="ArgumentNullException"><paramref name="webClient"/> is <see langword="null" />.</exception>
        public SignInViewModel(AnonymousFactorioWebClient webClient)
        {
            if (webClient == null)
                throw new ArgumentNullException("webClient");

            _webClient = webClient;

            this.WhenActivated(() =>
            {
                var disposer = new CompositeDisposable();

                var areFieldsValid = this.WhenAnyValue(
                        viewModel => viewModel.UsernameOrEmail,
                        viewModel => viewModel.Password)
                    .Select(fields => 
                        !string.IsNullOrWhiteSpace(fields.Item1) && 
                        !string.IsNullOrWhiteSpace(fields.Item2));

                // The low level factorio auth observable returns success or throws exception
                // Bad api choice? better would be for expected errors to return AuthResult.ErrorXYZ, unexpected errors call OnError()
                AttemptSignIn = ReactiveCommand.CreateAsyncTask(areFieldsValid, o => Task.FromResult(FactorioUserSession.FactorioAuthResult.None))
                    .AddTo(disposer);

                return disposer;
            });
        }

        public string UsernameOrEmail
        {
            get { return _usernameOrEmail; }
            set { this.RaiseAndSetIfChanged(ref _usernameOrEmail, value); }
        }

        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        public IReactiveCommand Cancel { get; }

        public ReactiveCommand<FactorioUserSession.FactorioAuthResult> AttemptSignIn { get; private set; }
    }
}
