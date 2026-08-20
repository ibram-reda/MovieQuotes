using Avalonia;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;

namespace MovieQuotes.UI
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            SetupLogging();
            Log.Logger.Information("------------------------------");

            BuildAvaloniaApp()
               .StartWithClassicDesktopLifetime(args);

        }

        private static void SetupLogging()
        {
            var logDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "Logging"));
            Directory.CreateDirectory(logDir);

            foreach (var file in Directory.EnumerateFiles(logDir, "MovieQ-*.log"))
            {
                try { File.Delete(file); } catch { /* Ignore delete errors */ }
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(Path.Combine(logDir, "MovieQ-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console()
                .CreateLogger();

            Trace.Listeners.Add(new SerilogTraceListener());
        }

        private class SerilogTraceListener : TraceListener
        {
            public override void Write(string? message) { }
            public override void WriteLine(string? message)
            {
                if (!string.IsNullOrEmpty(message) && message.StartsWith("["))
                    Log.Debug(message);
            }
        }
        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
#if DEBUG
                .WithDeveloperTools()
#endif
                .WithInterFont()
                .LogToTrace();
    }


}
