using Moq;
using Respatch.Models;
using Respatch.Services;
using Respatch.Stores;

namespace Respatch.Tests;

public class ProjectStoreTests
{
    private static (ProjectStore store, Mock<ISettingsService> mock) CreateStore(
        List<Project>? initialProjects = null,
        string? activeId = null)
    {
        var mock = new Mock<ISettingsService>();
        mock.Setup(s => s.GetProjects()).Returns(initialProjects ?? []);
        mock.Setup(s => s.GetActiveProjectId()).Returns(activeId);
        var store = new ProjectStore(mock.Object);
        return (store, mock);
    }

    [Fact]
    public void Constructor_LoadsProjectsFromSettings()
    {
        var projects = new List<Project>
        {
            new("id-1", "Project A", "https://a.com", "tok-a")
        };
        var (store, _) = CreateStore(projects);

        Assert.Single(store.Projects);
        Assert.Equal("id-1", store.Projects[0].Id);
    }

    [Fact]
    public void Constructor_LoadsActiveProjectIdFromSettings()
    {
        var projects = new List<Project> { new("id-1", "A", "https://a.com", "tok") };
        var (store, _) = CreateStore(projects, "id-1");

        Assert.Equal("id-1", store.ActiveProjectId);
    }

    [Fact]
    public void ActiveProject_ReturnsMatchingProject()
    {
        var projects = new List<Project> { new("id-1", "A", "https://a.com", "tok") };
        var (store, _) = CreateStore(projects, "id-1");

        Assert.NotNull(store.ActiveProject);
        Assert.Equal("id-1", store.ActiveProject!.Id);
    }

    [Fact]
    public void ActiveProject_ReturnsNull_WhenNoActiveId()
    {
        var (store, _) = CreateStore();
        Assert.Null(store.ActiveProject);
    }

    [Fact]
    public void HasActiveProject_ReturnsFalse_WhenNoActiveProject()
    {
        var (store, _) = CreateStore();
        Assert.False(store.HasActiveProject());
    }

    [Fact]
    public void HasActiveProject_ReturnsTrue_WhenActiveProjectSet()
    {
        var projects = new List<Project> { new("id-1", "A", "https://a.com", "tok") };
        var (store, _) = CreateStore(projects, "id-1");

        Assert.True(store.HasActiveProject());
    }

    [Fact]
    public void AddProject_AddsToCollectionAndPersists()
    {
        var (store, mock) = CreateStore();
        var project = new Project("id-new", "New", "https://new.com", "tok-new");

        store.AddProject(project);

        Assert.Single(store.Projects);
        mock.Verify(s => s.AddProject(It.Is<Project>(p => p.Id == "id-new")), Times.Once);
    }

    [Fact]
    public void RemoveProject_RemovesFromCollectionAndPersists()
    {
        var projects = new List<Project>
        {
            new("id-1", "A", "https://a.com", "tok-a"),
            new("id-2", "B", "https://b.com", "tok-b")
        };
        var (store, mock) = CreateStore(projects);

        store.RemoveProject("id-1");

        Assert.Single(store.Projects);
        Assert.Equal("id-2", store.Projects[0].Id);
        mock.Verify(s => s.RemoveProject("id-1"), Times.Once);
    }

    [Fact]
    public void RemoveProject_ClearsActiveId_WhenActiveProjectRemoved()
    {
        var projects = new List<Project> { new("id-1", "A", "https://a.com", "tok") };
        var (store, mock) = CreateStore(projects, "id-1");

        store.RemoveProject("id-1");

        Assert.Null(store.ActiveProjectId);
        mock.Verify(s => s.SetActiveProjectId(null), Times.Once);
    }

    [Fact]
    public void SetActiveProject_UpdatesActiveIdAndPersists()
    {
        var projects = new List<Project> { new("id-1", "A", "https://a.com", "tok") };
        var (store, mock) = CreateStore(projects);

        store.SetActiveProject("id-1");

        Assert.Equal("id-1", store.ActiveProjectId);
        mock.Verify(s => s.SetActiveProjectId("id-1"), Times.Once);
    }
}
