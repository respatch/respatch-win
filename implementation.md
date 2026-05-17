# Respatch Windows Port Implementation Plan

### Introduction
Respatch is a monitoring tool for message transports, originally developed for GNOME (TypeScript/GJS). It allows users to monitor the status of various message transports, view recent and failed messages, and perform administrative actions like retrying or removing failed messages.

The original GNOME version of the application is located in the root `app` in parent direcotry of this project

This document outlines the architecture and implementation roadmap for porting Respatch to a native Windows application using **C#, .NET, and WinUI 3**.

### Architecture Overview
The Windows version will follow the **MVVM (Model-View-ViewModel)** architectural pattern to ensure a clean separation of concerns, testability, and maintainability.

- **Models**: Plain C# classes (DTOs) representing the data from the Respatch API.
- **ViewModels**: Logic for managing application state, handling user interactions, and preparing data for the UI.
- **Views**: WinUI 3 XAML files defining the user interface.
- **Services**: Singleton or scoped services for cross-cutting concerns like API communication, logging, configuration, and notifications.

### Key Systems to Port
1.  **Localization**: Transition from `gettext` (.po/.mo) to Windows Resources (.resw).
2.  **Logging**: Implementation of a logging service using `Microsoft.Extensions.Logging`.
3.  **Configuration**: Migrating from GNOME's `GSettings` to Windows `LocalSettings` or a JSON-based configuration file.
4.  **API Client**: Porting the `gjsFetch` (Soup-based) client to .NET's `HttpClient`.
5.  **State Management**: Replacing GObject-based stores with an MVVM-compatible state management approach.
6.  **GUI**: Mapping GTK 4, Libadwaita, and Blueprint UI definitions to WinUI 3 XAML.
7.  **Notifications**: Porting GNOME's `Gio.Notification` to Windows Toast Notifications.

### Implementation Roadmap
The porting process is divided into several detailed steps:

1.  **[Step 1: Localization](step-01-localization.md)** - Setting up the resource-based translation system.
2.  **[Step 2: Logging](step-02-logging.md)** - Implementing the logging infrastructure.
3.  **[Step 3: Configuration](step-03-configuration.md)** - Setting up application settings and project storage.
4.  **[Step 4: API Client](step-04-api-client.md)** - Developing the HttpClient-based service and DTO models.
5.  **[Step 5: State Management](step-05-state-management.md)** - Implementing the ProjectService and MVVM foundations.
6.  **[Step 6: User Interface](step-06-ui.md)** - Porting the UI components and windows to XAML.
7.  **[Step 7: Notifications](step-07-notifications.md)** - Integrating with Windows Toast Notifications.
