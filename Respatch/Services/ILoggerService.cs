namespace Respatch.Services;

public interface ILoggerService
{
    bool IsEnabled { get; set; }
    bool IsLogToFile { get; set; }
    void Log(string message, LogLevel level = LogLevel.Info);
    void Info(string message);
    void Warning(string message);
    void Error(string message, Exception? exception = null);
}

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error
}
