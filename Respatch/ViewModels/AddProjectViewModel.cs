using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Respatch.Models;
using Respatch.Services;
using Respatch.Stores;

namespace Respatch.ViewModels;

public enum DialogState { Verify, Verifying, Add, Error }

/// <summary>
/// ViewModel for AddProjectDialog. Replaces AddProjectDialog.ts 3-state button machine.
/// States: Verify → Verifying → Add (success) / Error (failure).
/// Resets to Verify when any field changes.
/// </summary>
public partial class AddProjectViewModel : ObservableObject
{
    private readonly IApiClient _apiClient;
    private readonly ProjectStore _projectStore;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ActionButtonLabel))]
    [NotifyCanExecuteChangedFor(nameof(ActionCommand))]
    private DialogState _state = DialogState.Verify;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ActionCommand))]
    private bool _isBusy;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _url = string.Empty;

    [ObservableProperty]
    private string _token = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    public string ActionButtonLabel => State switch
    {
        DialogState.Verify  => "Overiť",
        DialogState.Verifying => "Overujem…",
        DialogState.Add     => "Uložiť",
        DialogState.Error   => "Skúsiť znova",
        _                   => "Overiť"
    };

    public bool CanExecuteAction => !IsBusy && !string.IsNullOrWhiteSpace(Name)
                                             && !string.IsNullOrWhiteSpace(Url)
                                             && !string.IsNullOrWhiteSpace(Token);

    public AddProjectViewModel(IApiClient apiClient, ProjectStore projectStore)
    {
        _apiClient = apiClient;
        _projectStore = projectStore;
    }

    [RelayCommand(CanExecute = nameof(CanExecuteAction))]
    private async Task ActionAsync()
    {
        if (State == DialogState.Add)
            SaveAndClose();
        else
            await VerifyAsync();
    }

    private async Task VerifyAsync()
    {
        State = DialogState.Verifying;
        IsBusy = true;
        HasError = false;
        ErrorMessage = string.Empty;

        try
        {
            var normalizedUrl = Url.TrimEnd('/');
            await _apiClient.VerifyProjectAsync(normalizedUrl, Token);
            State = DialogState.Add;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
            State = DialogState.Error;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SaveAndClose()
    {
        var project = new Project(
            Id: Guid.NewGuid().ToString(),
            Name: Name.Trim(),
            Url: Url.TrimEnd('/'),
            Token: Token.Trim());

        _projectStore.AddProject(project);
        _projectStore.SetActiveProject(project.Id);

        // Signal dialog to close via event
        ProjectSaved?.Invoke(this, project);
    }

    public event EventHandler<Project>? ProjectSaved;

    // Reset to Verify state when any field changes
    partial void OnNameChanged(string value) => ResetState();
    partial void OnUrlChanged(string value) => ResetState();
    partial void OnTokenChanged(string value) => ResetState();

    private void ResetState()
    {
        if (State is DialogState.Add or DialogState.Error)
        {
            State = DialogState.Verify;
            HasError = false;
            ErrorMessage = string.Empty;
        }
    }
}
