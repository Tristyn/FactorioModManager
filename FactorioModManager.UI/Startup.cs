using System;
using System.Reactive.Linq;
using System.Windows.Forms;
using FactorioModManager.UI.ViewModels;
using FactorioModManager.UI.Views;
using ReactiveUI;
using Splat;

namespace FactorioModManager.UI
{
    public static class Startup
    {
        private static Action<Delegate> _invokeOnMainThread;

        [STAThread]
        public static void Main(string[] args)
        {
            InitializeDependancyResolver();

            Application.EnableVisualStyles();
            var form = new DebugShellView();

            _invokeOnMainThread = action => form.Invoke(action);
            
            Application.Run(form);

            
        }

        public static void UIInvoke(Delegate action)
        {
            _invokeOnMainThread(action);
        }

        private static void InitializeDependancyResolver()
        {
            var container = Locator.CurrentMutable;

            container.InitializeSplat();
            container.InitializeReactiveUI();

            container.Register(() => new InstallationView(), typeof(IViewFor<InstallationViewModel>));
        }
    }
}
