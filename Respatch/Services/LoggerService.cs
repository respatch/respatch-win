using System.Diagnostics;

namespace Respatch.Services;

/// <summary>
/// Multi-transport logger. Replaces LoggerService.ts (ConsoleTransport + FileTransport).
/// Log file path: %LOCALAPPDATA%\Respatch\respatch.log
/// </summary>
public class LoggerService : ILoggerService
{
    private static readonly string LogFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Respatch",
        "respatch.log");

    private readonly object _fileLock = new();

    public bool IsEnabled { get; set; } = true;
    public bool IsLogToFile { get; set; } = false;

    public void Log(string message, LogLevel level = LogLevel.Info)
    {
        if (!IsEnabled) return;

        var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level.ToString().ToUpper()}] {message}";

        // Console transport
        Debug.WriteLine(entry);

        // File transport
        if (IsLogToFile)
            WriteToFile(entry);
    }

    public void Info(string message) => Log(message, LogLevel.Info);
    public void Warning(string message) => Log(message, LogLevel.Warning);
    public void Error(string message, Exception? exception = null)
    {
        var full = exception is null ? message : $"{message} | {exception.GetType().Name}: {exception.Message}";
        Log(full, LogLevel.Error);
    }

    private void WriteToFile(string entry)
    {
        try
        {
            lock (_fileLock)
            {
                var dir = Path.GetDirectoryName(LogFilePath)!;
                Directory.CreateDirectory(dir);
                File.AppendAllText(LogFilePath, entry + Environment.NewLine);
            }
        }
        catch
        {
            // Swallow file errors — logging must never crash the app
        }
    }
}
