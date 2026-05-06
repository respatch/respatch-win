using Moq;
using Respatch.Models;
using Respatch.Services;

namespace Respatch.Tests;

/// <summary>
/// Tests for ISettingsService contract using Moq.
/// The real SettingsService requires Windows.Storage (runtime-only),
/// so these tests verify the interface contract via a mock and an in-memory stub.
/// </summary>
public class SettingsServiceTests
{
    // ── In-memory stub for contract testing ───────────────────────────────────
    private sealed class InMemorySettingsService : ISettingsService
    {
        private readonly List<Project> _projects = [];
        private string? _activeProjectId;
        private bool _loggingEnabled = true;
        private bool _logToFile = false;

        public IReadOnlyList<Project> GetProjects() => _projects.AsReadOnly();
        public void AddProject(Project project) => _projects.Add(project);
        public void RemoveProject(string id) => _projects.RemoveAll(p => p.Id == id);
        public string? GetActiveProjectId() => _activeProjectId;
        public void SetActiveProjectId(string? id) => _activeProjectId = id;
        public bool GetLoggingEnabled() => _loggingEnabled;
        public void SetLoggingEnabled(bool value) => _loggingEnabled = value;
        public bool GetLogToFile() => _logToFile;
        public void SetLogToFile(bool value) => _logToFile = value;
    }

    private static ISettingsService CreateService() => new InMemorySettingsService();

    [Fact]
    public void GetProjects_InitiallyEmpty()
    {
        var svc = CreateService();
        Assert.Empty(svc.GetProjects());
    }

    [Fact]
    public void AddProject_ThenGetProjects_ReturnsProject()
    {
        var svc = CreateService();
        var project = new Project("id-1", "My Project", "https://example.com", "token123");

        svc.AddProject(project);

        Assert.Single(svc.GetProjects());
        Assert.Equal("id-1", svc.GetProjects()[0].Id);
    }

    [Fact]
    public void RemoveProject_RemovesCorrectProject()
    {
        var svc = CreateService();
        svc.AddProject(new Project("id-1", "Project A", "https://a.com", "tok-a"));
        svc.AddProject(new Project("id-2", "Project B", "https://b.com", "tok-b"));

        svc.RemoveProject("id-1");

        Assert.Single(svc.GetProjects());
        Assert.Equal("id-2", svc.GetProjects()[0].Id);
    }

    [Fact]
    public void ActiveProjectId_DefaultsToNull()
    {
        var svc = CreateService();
        Assert.Null(svc.GetActiveProjectId());
    }

    [Fact]
    public void SetActiveProjectId_ThenGet_ReturnsId()
    {
        var svc = CreateService();
        svc.SetActiveProjectId("id-1");
        Assert.Equal("id-1", svc.GetActiveProjectId());
    }

    [Fact]
    public void LoggingEnabled_DefaultsToTrue()
    {
        var svc = CreateService();
        Assert.True(svc.GetLoggingEnabled());
    }

    [Fact]
    public void SetLoggingEnabled_False_ThenGet_ReturnsFalse()
    {
        var svc = CreateService();
        svc.SetLoggingEnabled(false);
        Assert.False(svc.GetLoggingEnabled());
    }

    [Fact]
    public void LogToFile_DefaultsToFalse()
    {
        var svc = CreateService();
        Assert.False(svc.GetLogToFile());
    }

    [Fact]
    public void SetLogToFile_True_ThenGet_ReturnsTrue()
    {
        var svc = CreateService();
        svc.SetLogToFile(true);
        Assert.True(svc.GetLogToFile());
    }

    [Fact]
    public void Moq_VerifyAddProjectCalled()
    {
        var mock = new Mock<ISettingsService>();
        var project = new Project("id-1", "Test", "https://test.com", "tok");

        mock.Object.AddProject(project);

        mock.Verify(s => s.AddProject(It.Is<Project>(p => p.Id == "id-1")), Times.Once);
    }
}
