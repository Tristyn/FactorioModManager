using System;
using System.Windows.Forms;
using System.Windows.Threading;
using FactorioModManager.UI.Views;
using ReactiveUI;
using Splat;

namespace FactorioModManager.UI
{
    public static class Startup
    {
        static Startup()
        {
            // AAaaaaahhhhh M$FT
            IsInDesignMode = System.Reflection.Assembly.GetExecutingAssembly()
                 .Location.Contains("VisualStudio");
        }

        [STAThread]
        public static void Main()
        {
            // Initialize the dispatcher.
            // RxUI may throw up if the thread doesn't have a dispatcher.
            var disp = Dispatcher.CurrentDispatcher;

            InitializeDependancyResolver();

            Application.EnableVisualStyles();
            Application.Run(new DebugShellView());
        }

        private static void InitializeDependancyResolver()
        {
            var container = Locator.CurrentMutable;

            container.InitializeSplat();
            container.InitializeReactiveUI();

            container.Register(() => new ConsoleLogger(), typeof(ILogger));
        }

        public static bool IsInDesignMode { get; }
    }
}
