# Step 2: Logging

### Current Implementation (GNOME)
The GNOME version uses a custom `LoggerService`.
-   **Service**: `app/src/services/LoggerService.ts`.
-   **Transports**:
    -   `ConsoleTransport`: Logs to the standard console.
    -   `FileTransport`: Logs to a file using `Gio.File.append_to`.
-   **Features**: Supports log levels (`debug`, `info`, `warn`, `error`) and optional context objects (JSON).

### Windows Port Strategy
The Windows version will leverage the standard **Microsoft.Extensions.Logging** infrastructure.

-   **Interface**: Use `ILogger<T>` or `ILogger` throughout the application.
-   **Service**: Create a `LoggingService` (as a wrapper or configuration point) that sets up the `ILoggerFactory`.
-   **Providers**:
    -   **Console**: Use `AddConsole()` for development debugging.
    -   **Debug**: Use `AddDebug()` for output in the IDE.
    -   **File**: Since .NET doesn't have a built-in file logger in the base package, we can either use a lightweight library (like `Serilog` or `NLog`) or implement a simple `ILoggerProvider` that writes to a file in the `AppData` folder.

### Migration Tasks
1.  **Define Logger Interface**: Decide whether to use `ILogger` directly or a custom wrapper for simplicity.
2.  **Implement File Logger**: Create a simple file logging provider that saves logs to `%LOCALAPPDATA%\Respatch\logs`.
3.  **Integrate with DI**: Register the logging system in the application's Dependency Injection (DI) container.
4.  **Replace Calls**: Replace `this.logger.info(...)` calls with `_logger.LogInformation(...)`.
