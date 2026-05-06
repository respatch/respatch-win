using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Respatch.ViewModels;

namespace Respatch.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; }

    public MainPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<MainViewModel>();

        // Auto-refresh when page is loaded
        Loaded += async (_, _) => await ViewModel.RefreshCommand.ExecuteAsync(null);
    }
}
