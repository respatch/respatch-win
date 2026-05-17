# Step 5: State Management

### Current Implementation (GNOME)
The GNOME version uses a GObject-based store for state management.
-   **Service**: `app/src/stores/ProjectStore.ts`.
-   **Features**:
    -   Manages the list of projects (`Project[]`).
    -   Tracks the `active-project` ID.
    -   Uses GObject signals (`notify::active-project`) to inform the UI about state changes.
    -   Persists changes via `SettingsService`.

### Windows Port Strategy
The Windows version will use the **MVVM (Model-View-ViewModel)** pattern, which is the standard for WinUI 3 applications.

-   **ProjectService**: A singleton service that manages the core state (projects and active project). It will use `ObservableCollection<Project>` and provide events or property change notifications.
-   **ViewModels**: Each main view will have a corresponding ViewModel:
    -   `MainViewModel`: Holds data for the dashboard (transports, messages).
    -   `ManageServersViewModel`: Handles project list management (add, edit, delete).
    -   `SettingsViewModel`: Handles application-wide settings.
-   **Property Notifications**: Use `INotifyPropertyChanged` or the `ObservableObject` from the **CommunityToolkit.Mvvm** package.
-   **Dependency Injection**: Use a DI container (e.g., `Microsoft.Extensions.DependencyInjection`) to manage the lifecycle of services and provide them to ViewModels.

### Migration Tasks
1.  **Set up MVVM Foundation**: Include the `CommunityToolkit.Mvvm` NuGet package.
2.  **Implement ProjectService**: Port the logic from `ProjectStore.ts` to a C# service.
3.  **Create ViewModels**:
    -   Define properties and commands needed for the UI.
    -   Implement the polling logic for `MainViewModel` (refreshing data every 10 seconds).
4.  **Wire up DI**: Register services (`SettingsService`, `ApiClient`, `ProjectService`, `LoggerService`) and ViewModels in `App.xaml.cs`.
