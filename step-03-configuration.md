# Step 3: Configuration

### Current Implementation (GNOME)
The GNOME version uses `GSettings` for persistent configuration.
-   **Service**: `app/src/services/SettingsService.ts`.
-   **Schema**: `sk.mostka.Respatch`.
-   **Key Settings**:
    -   `projects`: An array of JSON strings representing `Project` objects.
    -   `last-active-project`: ID of the currently selected project.
    -   Logging preferences (`logging-enabled`, `log-to-file`, `log-path`).
    -   UI preferences (`messenger-dashboard-url`, `preferred-browser`).

### Windows Port Strategy
The Windows version will use a combination of **LocalSettings** for simple flags and a **JSON file** for complex data like the project list.

-   **Simple Settings**: Use `Windows.Storage.ApplicationData.Current.LocalSettings` for basic types (bool, string, int).
    -   Example: `active-project-id`, `logging-enabled`.
-   **Project List**: Store the `Project` objects in a `projects.json` file located in `ApplicationData.Current.LocalFolder`.
    -   This avoids the size limitations of `LocalSettings` and allows for easier manual editing or backup.
-   **Service**: Create a `SettingsService` in C# that abstracts these two storage mechanisms.

### Migration Tasks
1.  **Define Settings Model**: Create a C# class to represent the application settings.
2.  **Implement Storage Logic**:
    -   Use `System.Text.Json` for serializing/deserializing the projects list.
    -   Use `ApplicationDataContainer` for simple key-value pairs.
3.  **Port Project Management**: Re-implement `addProject`, `updateProject`, and `removeProject` logic from the TypeScript `SettingsService`.
4.  **Path Handling**: Ensure log paths and storage paths use Windows-standard locations (e.g., `%LOCALAPPDATA%`).
