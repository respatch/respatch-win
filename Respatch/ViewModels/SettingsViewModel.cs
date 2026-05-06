using CommunityToolkit.Mvvm.ComponentModel;
using Respatch.Services;

namespace Respatch.ViewModels;

/// <summary>
/// ViewModel for SettingsWindow. Replaces SettingsWindow.ts.
/// Mirrors sensitive: bind logging_enabled_row.active for IsLogToFileEnabled.
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settings;
    private readonly ILoggerService _logger;

    public static string LogFilePath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Respatch",
        "respatch.log");

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLogToFileEnabled))]
    private bool _isLoggingEnabled;

    [ObservableProperty]
    private bool _isLogToFile;

    // Log-to-file toggle is disabled when logging is off (mirrors GNOME sensitive binding)
    public bool IsLogToFileEnabled => IsLoggingEnabled;

    public SettingsViewModel(ISettingsService settings, ILoggerService logger)
    {
        _settings = settings;
        _logger = logger;

        _isLoggingEnabled = _settings.GetLoggingEnabled();
        _isLogToFile = _settings.GetLogToFile();
    }

    partial void OnIsLoggingEnabledChanged(bool value)
    {
        _settings.SetLoggingEnabled(value);
        _logger.IsEnabled = value;
        OnPropertyChanged(nameof(IsLogToFileEnabled));
    }

    partial void OnIsLogToFileChanged(bool value)
    {
        _settings.SetLogToFile(value);
        _logger.IsLogToFile = value;
    }
}
