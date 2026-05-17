# Step 6: User Interface

### Current Implementation (GNOME)
The GNOME version uses GTK 4 and Libadwaita, with UI definitions often written in Blueprint.
-   **Layouts**: Uses `Adw.Window`, `Adw.HeaderBar`, `Gtk.ListBox`, and `Adw.ActionRow`.
-   **Windows**:
    -   `MainWindow.ts`: Dashboard with 4 main lists (Failed Transports, Active Transports, Recent Messages, Failed Messages).
    -   `ManageServersWindow.ts`: List of servers with add/edit/delete functionality.
    -   `SettingsWindow.ts`: Basic application settings.
    -   `WelcomeWindow.ts`: Initial setup screen.
-   **Widgets**: Custom rows for each message/transport type.

### Windows Port Strategy
The Windows version will use **WinUI 3 (Windows UI Library)** and **XAML**.

-   **Windows & Navigation**:
    -   `MainWindow.xaml`: Primary window. Consider using a `NavigationView` if adding more sections later, but for now, a layout matching the GNOME original is preferred.
    -   `ContentDialog`: Use for "Add Server" and "Settings" to maintain a modern Windows feel.
-   **Data Binding**: Use `{x:Bind}` for high-performance binding to ViewModels.
-   **Lists**: Use `ListView` or `ItemsControl` with custom `DataTemplate` for rows.
    -   `TransportTemplate`, `MessageTemplate`, etc.
-   **Styling**: Use WinUI 3's built-in styles and `ThemeResource` for light/dark mode support (matching Windows system settings).

### Migration Tasks
1.  **Map UI Components**:
    -   `Gtk.ListBox` -> `ListView`.
    -   `Adw.ActionRow` -> `StackPanel` or `Grid` inside a `DataTemplate`.
    -   `Adw.HeaderBar` -> Custom `Grid` or `CommandBar`.
2.  **Create XAML Views**: Build the `MainWindow`, `ManageServersWindow`, and `SettingsWindow` using WinUI 3 components.
3.  **Implement Data Templates**: Create `UserControl` or `DataTemplate` resources for the various row types.
4.  **Polish Layout**: Ensure the application follows **Windows Design Guidelines** (Fluent Design) while preserving the core functionality and density of the GNOME version.
