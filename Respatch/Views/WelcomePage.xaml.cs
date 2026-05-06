using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Respatch.ViewModels;

namespace Respatch.Views;

public sealed partial class WelcomePage : Page
{
    public WelcomeViewModel ViewModel { get; }

    public WelcomePage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<WelcomeViewModel>();
    }
}
