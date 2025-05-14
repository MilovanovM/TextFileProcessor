namespace TestFileGenerator.Tests;

public abstract class TestBase : IDisposable
{
    private readonly string _testDirectory;
    private readonly List<string> _tempFiles = [];

    protected TestBase()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "TestFileGeneratorTests");
        Directory.CreateDirectory(_testDirectory);
    }

    public void Dispose()
    {
        foreach (var file in _tempFiles)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch { /* Ignore cleanup errors */ }
        }

        try
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
        catch { /* Ignore cleanup errors */ }
    }

    protected string GetTempFilePath()
    {
        var path = Path.Combine(_testDirectory, Path.GetRandomFileName());
        _tempFiles.Add(path);
        return path;
    }

    protected void CreateTestFile(string path, string[] lines)
    {
        File.WriteAllLines(path, lines);
    }
} 