# Step 7: Notifications

### Current Implementation (GNOME)
The GNOME version uses the `Gio.Notification` API for system alerts.
-   **Service**: `app/src/services/NotificationService.ts`.
-   **Logic**:
    -   Tracks previously notified message IDs to avoid duplicates.
    -   **Critical Threshold**: If more than 5 new failures appear in one poll, it shows a single summary notification instead of individual ones.
    -   **Cooldown**: Prevents spamming critical notifications.
    -   **Buttons**: "Mute for 1 hour", "Mute for 1 day", "Turn off".
-   **Daemon Awareness**: Checks via D-Bus if `sk.tito10047.respatch.Daemon` is running; if so, the UI disables its own local notifications to avoid duplicates.

### Windows Port Strategy
The Windows version will use the **Windows Toast Notification** system (part of the Windows App SDK).

-   **Service**: Create a `NotificationService` in C#.
-   **Implementation**: Use the `Microsoft.Windows.AppNotifications` namespace.
-   **Visuals**: Use the `AppNotificationBuilder` to construct toasts with titles, bodies, and interactive buttons.
-   **Background Tasks**: (Optional) Consider if a background task is needed for notifications when the app is closed, similar to the GNOME daemon. For the initial port, focusing on notifications while the app is running is sufficient.

### Migration Tasks
1.  **Initialize Toast System**: Register the app for notifications in `App.xaml.cs`.
2.  **Port Notification Logic**: Re-implement the deduplication and critical threshold logic from `NotificationService.ts`.
3.  **Implement Actions**: Handle button clicks (e.g., Mute) using toast activation arguments.
4.  **Polish Content**: Ensure notification text is localized using the system established in Step 1.
