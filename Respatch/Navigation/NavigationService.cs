using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Respatch.Views;
using Respatch.Views.Dialogs;

namespace Respatch.Navigation;

/// <summary>
/// Central navigation coordinator. Replaces WindowManager.ts.
/// Owns the single MainWindow and swaps pages inside its root Frame.
/// </summary>
public class NavigationService
{
    private MainWindow? _mainWindow;
    private Frame? _rootFrame;

    public MainWindow MainWindow => _mainWindow
        ?? throw new InvalidOperationException("NavigationService not initialized. Call Initialize() first.");

    public void Initialize()
    {
        _mainWindow = new MainWindow();
        _rootFrame = _mainWindow.RootFrame;
        _mainWindow.Activate();
    }

    public void NavigateToWelcome()
    {
        EnsureInitialized();
        _rootFrame!.Navigate(typeof(WelcomePage));
    }

    public void NavigateToMain()
    {
        EnsureInitialized();
        _rootFrame!.Navigate(typeof(MainPage));
    }

    public async Task ShowAddProjectDialogAsync()
    {
        EnsureInitialized();
        var dialog = new AddProjectDialog
        {
            XamlRoot = _mainWindow!.Content.XamlRoot
        };
        await dialog.ShowAsync();
    }

    public async Task ShowSettingsAsync()
    {
        EnsureInitialized();
        var settingsWindow = new SettingsWindow();
        settingsWindow.Activate();
        await Task.CompletedTask;
    }

    private void EnsureInitialized()
    {
        if (_mainWindow is null)
            throw new InvalidOperationException("NavigationService not initialized. Call Initialize() first.");
    }
}
