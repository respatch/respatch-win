using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Respatch.ViewModels;

namespace Respatch.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<SettingsViewModel>();
    }
}
