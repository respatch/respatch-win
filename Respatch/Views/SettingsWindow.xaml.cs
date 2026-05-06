using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;

namespace Respatch.Views;

/// <summary>
/// Settings window opened as a child window. Replaces settings.window.set_transient_for(parent).
/// </summary>
public sealed partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        TrySetMicaBackdrop();
        ExtendsContentIntoTitleBar = true;
        SettingsFrame.Navigate(typeof(SettingsPage));

        // Set a reasonable size for the settings window
        AppWindow.Resize(new Windows.Graphics.SizeInt32(560, 480));
    }

    private void TrySetMicaBackdrop()
    {
        if (MicaController.IsSupported())
            SystemBackdrop = new MicaBackdrop();
        else if (DesktopAcrylicController.IsSupported())
            SystemBackdrop = new DesktopAcrylicBackdrop();
    }
}
