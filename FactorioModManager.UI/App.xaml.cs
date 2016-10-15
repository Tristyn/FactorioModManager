﻿using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using FactorioModManager.UI.Views;
using Serilog;

namespace FactorioModManager.UI
{
    class App : Application
    {

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }

        [STAThread]
        static void Main(string[] args)
        {
            InitializeLogging();
            AppBuilder.Configure<App>()
                .UseWin32()
                .UseDirect2D1()
                .Start<DebugView>();
        }

        public static void AttachDevTools(Window window)
        {
#if DEBUG
            DevTools.Attach(window);
#endif
        }

        private static void InitializeLogging()
        {
#if DEBUG
            SerilogLogger.Initialize(new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Trace(outputTemplate: "{Area}: {Message}")
                .CreateLogger());
#endif
        }
    }
}