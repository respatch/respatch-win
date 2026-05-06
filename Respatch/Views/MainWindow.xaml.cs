using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Respatch.Views;

public sealed partial class MainWindow : Window
{
    public Frame RootFrame => (Frame)Content;

    public MainWindow()
    {
        InitializeComponent();
        TrySetMicaBackdrop();
        ExtendsContentIntoTitleBar = true;
    }

    private void TrySetMicaBackdrop()
    {
        if (MicaController.IsSupported())
        {
            SystemBackdrop = new MicaBackdrop();
        }
        else if (DesktopAcrylicController.IsSupported())
        {
            SystemBackdrop = new DesktopAcrylicBackdrop();
        }
    }
}
