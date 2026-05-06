using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Respatch.Navigation;
using Respatch.Services;
using Respatch.Stores;
using Respatch.ViewModels;

namespace Respatch;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public App()
    {
        InitializeComponent();
        Services = ConfigureServices();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // HTTP
        services.AddSingleton<HttpClient>();

        // Services
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<ILoggerService, LoggerService>();
        services.AddSingleton<IApiClient, ApiClient>();

        // Stores
        services.AddSingleton<ProjectStore>();

        // Navigation
        services.AddSingleton<NavigationService>();

        // ViewModels
        services.AddTransient<WelcomeViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<AddProjectViewModel>();

        return services.BuildServiceProvider();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var navigationService = Services.GetRequiredService<NavigationService>();
        var projectStore = Services.GetRequiredService<ProjectStore>();

        navigationService.Initialize();

        if (projectStore.HasActiveProject())
            navigationService.NavigateToMain();
        else
            navigationService.NavigateToWelcome();
    }
}
