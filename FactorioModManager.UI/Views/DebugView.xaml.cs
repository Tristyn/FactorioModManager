using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FactorioModManager.UI.Views
{
    public class DebugView : Window
    {
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
