using SelectAid.Input;
using SelectAid.Persistence;
using SelectAid.Theme;

namespace SelectAid.Services;

public static class AppServices
{
    public static PersistenceService Persistence { get; private set; } = null!;
    public static SpeechService Speech { get; private set; } = null!;
    public static InputRouter InputRouter { get; private set; } = null!;
    public static InputTimingController TimingController { get; private set; } = null!;
    public static ThemeService Theme { get; private set; } = null!;
    public static MetricsService Metrics { get; private set; } = null!;
    public static StartupService Startup { get; private set; } = null!;
    public static InputSendService InputSend { get; private set; } = null!;

    public static void Initialize()
    {
        Persistence = new PersistenceService();
        InputSend = new InputSendService();
        Speech = new SpeechService();
        InputRouter = new InputRouter();

        Persistence.Initialize();
        TimingController = new InputTimingController(InputRouter);
        Theme = new ThemeService();
        Metrics = new MetricsService();
        Startup = new StartupService();
        Theme.ApplyTheme(Persistence.Settings.CurrentProfile.ThemeId, Persistence.Settings.HighContrast);
        InputRouter.SetMode(Persistence.Settings.CurrentInputMode);
        Startup.SetAutoStart(Persistence.Settings.AutoStartEnabled);
    }
}
