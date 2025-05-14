namespace TestFileGenerator.Simple.Tests;

public class FileGenerationTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly List<string> _tempFiles = [];

    public FileGenerationTests()
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

    private string GetTempFilePath()
    {
        var path = Path.Combine(_testDirectory, Path.GetRandomFileName());
        _tempFiles.Add(path);
        return path;
    }

    [Fact]
    public void GenerateTestFile_CreatesFileWithCorrectSize()
    {
        // Arrange
        var outputPath = GetTempFilePath();
        const long fileSizeBytes = 1024L * 1024L;

        // Act
        Program.WriteRandomLinesToFile(outputPath, fileSizeBytes);

        // Assert
        var fileInfo = new FileInfo(outputPath);
        Assert.True(fileInfo.Exists);
        Assert.True(fileInfo.Length >= fileSizeBytes);
    }

    [Fact]
    public void GenerateTestFile_ContainsValidContent()
    {
        // Arrange
        var outputPath = GetTempFilePath();
        const int fileSizeMb = 1;

        // Act
        Program.WriteRandomLinesToFile(outputPath, fileSizeMb);

        // Assert
        var lines = File.ReadAllLines(outputPath);
        Assert.NotEmpty(lines);
        
        foreach (var line in lines)
        {
            var parts = line.Split(". ", 2);
            Assert.Equal(2, parts.Length);
            Assert.True(int.TryParse(parts[0], out _));
            Assert.NotEmpty(parts[1]);
        }
    }
} 