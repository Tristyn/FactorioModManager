using System.Reactive.Disposables;
using System.Windows.Forms;
using FactorioModManager.UI.Extensions;
using FactorioModManager.UI.ViewModels;
using ReactiveUI;

namespace FactorioModManager.UI.Views
{
    public partial class SignInView : Form, IViewFor<SignInViewModel>
    {
        public SignInView(SignInViewModel vm)
            : this()
        {
            this.WhenActivated(() =>
            {
                var disposer = new CompositeDisposable();

                this.Bind(ViewModel, viewModel => viewModel.UsernameOrEmail, view => view.UsernameOrEmail.Text)
                    .AddTo(disposer);

                this.Bind(ViewModel, viewModel => viewModel.Password, view => view.Password.Text)
                    .AddTo(disposer);

                this.BindCommand(ViewModel, viewModel => viewModel.AttemptSignIn, view => view.SignInBtn)
                    .AddTo(disposer);

                return disposer;
            });

            ViewModel = vm;
        }

        public SignInView()
        {
            InitializeComponent();
        }

        #region IViewFor<MyViewModel>
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (SignInViewModel)value; }
        }

        public SignInViewModel ViewModel { get; set; }
        #endregion  
    }
}
