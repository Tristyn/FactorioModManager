using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FactorioModManager.UI.ViewModels;
using ReactiveUI;

namespace FactorioModManager.UI.Views
{
    class DebugView : Window, IViewFor<DebugViewModel>
    {
        object IViewFor.ViewModel { get; set; }
        public DebugViewModel ViewModel { get; set; }

        public DebugView()
        {
            this.InitializeComponent();
            App.AttachDevTools(this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
