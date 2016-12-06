using System;
using System.Windows.Forms;
using FactorioModManager.Lib.Web;
using FactorioModManager.UI.ViewModels;

namespace FactorioModManager.UI.Views
{
    class SignInDialog : IDisposable
    {
        private readonly SignInView _form;

        private bool _disposed;

        /// <exception cref="ArgumentNullException"><paramref name="webClient"/> is <see langword="null" />.</exception>
        public SignInDialog(AnonymousFactorioWebClient webClient)
        {
            if (webClient == null)
                throw new ArgumentNullException("webClient");

            _form = new SignInView(new SignInViewModel(webClient));
        }

        /// <summary>
        /// Shows the form and blocks the ui. This method returns when the form is closed.
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        public void ShowDialog(IWin32Window owner = null)
        {
            ThrowDisposed();

            if (owner != null)
                _form.ShowDialog(owner);
            else
                _form.ShowDialog();
        }

        /// <exception cref="ObjectDisposedException"></exception>
        void ThrowDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        public void Dispose()
        {
            _disposed = true;
            _form.Dispose();
        }
    }
}
