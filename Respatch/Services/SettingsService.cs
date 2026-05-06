using System.Text.Json;
using Respatch.Models;
using Windows.Storage;

namespace Respatch.Services;

/// <summary>
/// Wraps ApplicationData.Current.LocalSettings. Replaces SettingsService.ts (Gio.Settings).
/// </summary>
public class SettingsService : ISettingsService
{
    private const string KeyProjects = "projects";
    private const string KeyActiveProject = "last-active-project";
    private const string KeyLoggingEnabled = "logging-enabled";
    private const string KeyLogToFile = "log-to-file";

    private readonly ApplicationDataContainer _settings;

    public SettingsService()
    {
        _settings = ApplicationData.Current.LocalSettings;
        InitializeDefaults();
    }

    private void InitializeDefaults()
    {
        if (!_settings.Values.ContainsKey(KeyLoggingEnabled))
            _settings.Values[KeyLoggingEnabled] = true;
        if (!_settings.Values.ContainsKey(KeyLogToFile))
            _settings.Values[KeyLogToFile] = false;
        if (!_settings.Values.ContainsKey(KeyProjects))
            _settings.Values[KeyProjects] = "[]";
    }

    public IReadOnlyList<Project> GetProjects()
    {
        var json = _settings.Values[KeyProjects] as string ?? "[]";
        return JsonSerializer.Deserialize<List<Project>>(json) ?? [];
    }

    public void AddProject(Project project)
    {
        var projects = GetProjects().ToList();
        projects.Add(project);
        _settings.Values[KeyProjects] = JsonSerializer.Serialize(projects);
    }

    public void RemoveProject(string id)
    {
        var projects = GetProjects().Where(p => p.Id != id).ToList();
        _settings.Values[KeyProjects] = JsonSerializer.Serialize(projects);
    }

    public string? GetActiveProjectId()
        => _settings.Values[KeyActiveProject] as string;

    public void SetActiveProjectId(string? id)
        => _settings.Values[KeyActiveProject] = id;

    public bool GetLoggingEnabled()
        => _settings.Values[KeyLoggingEnabled] is bool b ? b : true;

    public void SetLoggingEnabled(bool value)
        => _settings.Values[KeyLoggingEnabled] = value;

    public bool GetLogToFile()
        => _settings.Values[KeyLogToFile] is bool b ? b : false;

    public void SetLogToFile(bool value)
        => _settings.Values[KeyLogToFile] = value;
}
