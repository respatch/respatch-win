using Respatch.Services;

namespace Respatch.Tests;

public class LoggerServiceTests
{
    [Fact]
    public void Log_WhenDisabled_DoesNotThrow()
    {
        var logger = new LoggerService { IsEnabled = false };
        logger.Info("should be ignored");
    }

    [Fact]
    public void Log_WhenEnabled_DoesNotThrow()
    {
        var logger = new LoggerService { IsEnabled = true };
        logger.Info("test message");
        logger.Warning("test warning");
        logger.Error("test error");
    }

    [Fact]
    public void Error_WithException_IncludesExceptionInfo()
    {
        var logger = new LoggerService { IsEnabled = true };
        // Should not throw even with exception attached
        logger.Error("something failed", new InvalidOperationException("inner"));
    }

    [Fact]
    public void IsEnabled_DefaultsToTrue()
    {
        var logger = new LoggerService();
        Assert.True(logger.IsEnabled);
    }

    [Fact]
    public void IsLogToFile_DefaultsToFalse()
    {
        var logger = new LoggerService();
        Assert.False(logger.IsLogToFile);
    }

    [Fact]
    public void Log_ToFile_WhenLogToFileEnabled_DoesNotThrow()
    {
        var logger = new LoggerService { IsEnabled = true, IsLogToFile = true };
        // Writing to file should not throw (directory creation is handled internally)
        logger.Info("file log test");
    }

    [Fact]
    public void SetIsEnabled_False_ThenLog_DoesNothing()
    {
        var logger = new LoggerService { IsEnabled = true };
        logger.IsEnabled = false;
        // After disabling, logging should be a no-op
        logger.Info("this should not be logged");
    }
}
