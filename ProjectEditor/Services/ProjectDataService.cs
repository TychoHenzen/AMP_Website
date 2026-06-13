using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectEditor.Services;

public class ProjectDataService
{
    private readonly string _jsonPath;
    private readonly string _wwwrootProjectsPath;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public ProjectDataService()
    {
        // Release layout: AppContext.BaseDirectory = bin/Debug/net8.0/ → 4 levels up to repo root
        var fromBase = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../AutoARPG_WebAsm/wwwroot/Projects/projects.json"));

        // dotnet run fallback: CWD = ProjectEditor/
        var fromCwd = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "../AutoARPG_WebAsm/wwwroot/Projects/projects.json"));

        _jsonPath = File.Exists(fromBase) ? fromBase : fromCwd;
        _wwwrootProjectsPath = Path.GetDirectoryName(_jsonPath)!;
    }

    public List<ProjectInfo> GetProjects()
    {
        if (!File.Exists(_jsonPath))
            return new List<ProjectInfo>();

        var json = File.ReadAllText(_jsonPath);
        return JsonSerializer.Deserialize<List<ProjectInfo>>(json, SerializerOptions)
               ?? new List<ProjectInfo>();
    }

    /// <summary>Atomic write: write to .tmp file then replace the original.</summary>
    public void SaveProjects(List<ProjectInfo> projects)
    {
        var json = JsonSerializer.Serialize(projects, SerializerOptions);
        var tmpPath = _jsonPath + ".tmp";

        try
        {
            File.WriteAllText(tmpPath, json);
            File.Move(tmpPath, _jsonPath, overwrite: true);
        }
        catch
        {
            if (File.Exists(tmpPath))
                File.Delete(tmpPath);
            throw;
        }
    }

    public void MoveUp(List<ProjectInfo> projects, int index)
    {
        if (index <= 0 || index >= projects.Count) return;
        (projects[index - 1], projects[index]) = (projects[index], projects[index - 1]);
    }

    public void MoveDown(List<ProjectInfo> projects, int index)
    {
        if (index < 0 || index >= projects.Count - 1) return;
        (projects[index], projects[index + 1]) = (projects[index + 1], projects[index]);
    }

    public string GetProjectsBasePath() => _wwwrootProjectsPath;
}
