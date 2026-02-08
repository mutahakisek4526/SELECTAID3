using System.IO;
using System.Windows;
using System.Windows.Threading;
using SelectAid.Services;

namespace SelectAid;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception exception)
            {
                LoggingService.LogError("Unhandled exception", exception);
            }
        };

        DispatcherUnhandledException += OnDispatcherUnhandledException;
        System.Threading.Tasks.TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        Directory.CreateDirectory(AppPaths.AppDataDirectory);
        AppServices.Initialize();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LoggingService.LogError("Dispatcher exception", e.Exception);
        e.Handled = true;
    }

    private void OnUnobservedTaskException(object? sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
    {
        LoggingService.LogError("Unobserved task exception", e.Exception);
        e.SetObserved();
    }
}
