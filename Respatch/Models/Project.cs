namespace Respatch.Models;

/// <summary>
/// Represents a monitored Symfony Messenger project. Replaces models/Project.ts.
/// </summary>
public record Project(
    string Id,
    string Name,
    string Url,
    string Token
);
