﻿using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;
using FactorioModManager.UI.Extensions;
using FactorioModManager.UI.ViewModels;
using ReactiveUI;

namespace FactorioModManager.UI.Views
{
    public partial class DebugShellView : Form, IViewFor<DebugViewModel>
    {
        public DebugShellView()
        {
            if (Startup.IsInDesignMode)
                return;

            AutoScaleMode = AutoScaleMode.Font;
            InitializeComponent();
            
            Observable.FromEventPattern(
                ev => InstallationsList.SelectedValueChanged += ev,
                ev => InstallationsList.SelectedValueChanged -= ev)
                .Select(x => InstallationsList.SelectedItem)
                .BindTo(ViewModel, x => x.SelectedInstallationSpec);

            this.OneWayBind(ViewModel, x => x.Installations, x => x.InstallationsList.DataSource);

            this.OneWayBind(ViewModel, viewModel => viewModel.Installation, view => view.InstallationView.ViewModel);

            ViewModel = new DebugViewModel();
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
