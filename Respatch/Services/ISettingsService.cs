using Respatch.Models;

namespace Respatch.Services;

public interface ISettingsService
{
    IReadOnlyList<Project> GetProjects();
    void AddProject(Project project);
    void RemoveProject(string id);
    string? GetActiveProjectId();
    void SetActiveProjectId(string? id);
    bool GetLoggingEnabled();
    void SetLoggingEnabled(bool value);
    bool GetLogToFile();
    void SetLogToFile(bool value);
}
