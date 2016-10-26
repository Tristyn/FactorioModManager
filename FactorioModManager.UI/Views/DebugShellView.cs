using System.Reactive.Linq;
using System.Windows.Forms;
using FactorioModManager.UI.ViewModels;
using ReactiveUI;

namespace FactorioModManager.UI.Views
{
    public partial class DebugShellView : Form, IViewFor<DebugViewModel>
    {
        public DebugShellView()
        {
            AutoScaleMode = AutoScaleMode.Font;
            InitializeComponent();
            
            ViewModel = new DebugViewModel();

            Observable.FromEventPattern(
                ev => InstallationsList.SelectedValueChanged += ev,
                ev => InstallationsList.SelectedValueChanged -= ev)
                .Select(x => InstallationsList.SelectedItem)
                .BindTo(ViewModel, x => x.SelectedInstallationSpec);
            
            this.Bind(ViewModel, x => x.Installations, x => x.InstallationsList.DataSource);
            
            this.OneWayBind(ViewModel, viewModel => viewModel.Installation, view => view.InstallationView.ViewModel);
        }

        #region IViewFor<MyViewModel>
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (DebugViewModel)value; }
        }

        public DebugViewModel ViewModel { get; set; }
        #endregion
    }
}
