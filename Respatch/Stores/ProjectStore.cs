using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Respatch.Models;
using Respatch.Services;

namespace Respatch.Stores;

/// <summary>
/// Application-level state store for projects. Replaces ProjectStore.ts (GObject).
/// Loads from and persists to ISettingsService on every mutation.
/// </summary>
public partial class ProjectStore : ObservableObject
{
    private readonly ISettingsService _settings;

    [ObservableProperty]
    private ObservableCollection<Project> _projects = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ActiveProject))]
    [NotifyPropertyChangedFor(nameof(HasActiveProjectProperty))]
    private string? _activeProjectId;

    public ProjectStore(ISettingsService settings)
    {
        _settings = settings;
        Load();
    }

    private void Load()
    {
        var saved = _settings.GetProjects();
        Projects = new ObservableCollection<Project>(saved);
        ActiveProjectId = _settings.GetActiveProjectId();
    }

    public Project? ActiveProject
        => Projects.FirstOrDefault(p => p.Id == ActiveProjectId);

    // Exposed as a property so XAML x:Bind can observe it
    public bool HasActiveProjectProperty
        => ActiveProject is not null;

    public bool HasActiveProject() => ActiveProject is not null;

    public void AddProject(Project project)
    {
        _settings.AddProject(project);
        Projects.Add(project);
    }

    public void RemoveProject(string id)
    {
        _settings.RemoveProject(id);
        var existing = Projects.FirstOrDefault(p => p.Id == id);
        if (existing is not null)
            Projects.Remove(existing);

        if (ActiveProjectId == id)
            SetActiveProject(null);
    }

    public void SetActiveProject(string? id)
    {
        _settings.SetActiveProjectId(id);
        ActiveProjectId = id;
    }

    partial void OnActiveProjectIdChanged(string? value)
    {
        OnPropertyChanged(nameof(ActiveProject));
        OnPropertyChanged(nameof(HasActiveProjectProperty));
    }
}
