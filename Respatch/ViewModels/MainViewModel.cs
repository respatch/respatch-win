using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Respatch.Navigation;
using Respatch.Services;
using Respatch.Stores;

namespace Respatch.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IApiClient _apiClient;
    private readonly ProjectStore _projectStore;
    private readonly NavigationService _navigation;

    [ObservableProperty]
    private ObservableCollection<TransportInfo> _transports = [];

    [ObservableProperty]
    private ObservableCollection<ProcessedMessageSummary> _failedMessages = [];

    [ObservableProperty]
    private ObservableCollection<ProcessedMessageSummary> _recentMessages = [];

    [ObservableProperty]
    private string _activeProjectName = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    public MainViewModel(IApiClient apiClient, ProjectStore projectStore, NavigationService navigation)
    {
        _apiClient = apiClient;
        _projectStore = projectStore;
        _navigation = navigation;

        ActiveProjectName = _projectStore.ActiveProject?.Name ?? string.Empty;
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        var project = _projectStore.ActiveProject;
        if (project is null) return;

        IsLoading = true;
        HasError = false;
        ErrorMessage = string.Empty;

        try
        {
            var workersTask = _apiClient.GetWorkersAsync(project.Url, project.Token);
            var dashboardTask = _apiClient.GetDashboardAsync(project.Url, project.Token);
            var historyTask = _apiClient.GetHistoryAsync(project.Url, project.Token);

            await Task.WhenAll(workersTask, dashboardTask, historyTask);

            var dashboard = await dashboardTask;
            var history = await historyTask;

            Transports = new ObservableCollection<TransportInfo>(dashboard.Transports);
            FailedMessages = new ObservableCollection<ProcessedMessageSummary>(
                history.Messages.Where(m => m.Failed));
            RecentMessages = new ObservableCollection<ProcessedMessageSummary>(
                history.Messages.Where(m => !m.Failed));
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RetryMessageAsync(ProcessedMessageSummary message)
    {
        var project = _projectStore.ActiveProject;
        if (project is null) return;

        try
        {
            await _apiClient.RetryFailedMessageAsync(project.Url, project.Token,
                message.Transport, message.Id);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        await _navigation.ShowSettingsAsync();
    }
}
