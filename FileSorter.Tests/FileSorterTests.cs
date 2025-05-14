namespace FileSorter.Tests;

public class FileSorterTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly string _tempDirectory;

    public FileSorterTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "FileSorterTests");
        _tempDirectory = Path.Combine(_testDirectory, "Temp");
        Directory.CreateDirectory(_testDirectory);
        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }

    [Fact]
    public async Task SortFile_WithValidInput_SortsCorrectly()
    {
        // Arrange
        var inputPath = Path.Combine(_testDirectory, "input.txt");
        var outputPath = Path.Combine(_testDirectory, "output.txt");
        var inputContent = new[]
        {
            "415. Apple",
            "30432. Something something something",
            "1. Apple",
            "32. Cherry is the best",
            "2. Banana is yellow"
        };
        File.WriteAllLines(inputPath, inputContent);

        var expectedContent = new[]
        {
            "1. Apple",
            "415. Apple",
            "2. Banana is yellow",
            "32. Cherry is the best",
            "30432. Something something something"
        };

        // Act
        var sorter = new FileSorter(new FileSorterOptions { TempDirectory = _tempDirectory });
        await sorter.SortFile(inputPath, outputPath);

        // Assert
        var actualContent = await File.ReadAllLinesAsync(outputPath, TestContext.Current.CancellationToken);
        Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task SortFile_WithEmptyFile_CreatesEmptyOutput()
    {
        // Arrange
        var inputPath = Path.Combine(_testDirectory, "empty.txt");
        var outputPath = Path.Combine(_testDirectory, "empty_output.txt");
        await File.WriteAllTextAsync(inputPath, string.Empty, TestContext.Current.CancellationToken);

        // Act
        var sorter = new FileSorter(new FileSorterOptions { TempDirectory = _tempDirectory });
        await sorter.SortFile(inputPath, outputPath);

        // Assert
        var content = await File.ReadAllLinesAsync(outputPath, TestContext.Current.CancellationToken);
        Assert.Empty(content);
    }

    [Fact]
    public async Task SortFile_WithInvalidLines_SkipsInvalidLines()
    {
        // Arrange
        var inputPath = Path.Combine(_testDirectory, "invalid.txt");
        var outputPath = Path.Combine(_testDirectory, "invalid_output.txt");
        var inputContent = new[]
        {
            "415. Apple",
            "Invalid line",
            "1. Apple",
            "No number here",
            "2. Banana is yellow"
        };
        File.WriteAllLines(inputPath, inputContent);

        var expectedContent = new[]
        {
            "1. Apple",
            "415. Apple",
            "2. Banana is yellow"
        };

        // Act
        var sorter = new FileSorter(new FileSorterOptions { TempDirectory = _tempDirectory });
        await sorter.SortFile(inputPath, outputPath);

        // Assert
        var actualContent = await File.ReadAllLinesAsync(outputPath, TestContext.Current.CancellationToken);
        Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task SortFile_WithLargeFile_SortsCorrectly()
    {
        // Arrange
        var inputPath = Path.Combine(_testDirectory, "large.txt");
        var outputPath = Path.Combine(_testDirectory, "large_output.txt");
        
        // Create a large file with 100,000 lines
        var lines = Enumerable.Range(1, 100000)
            .Select(i => $"{i}. Test string {i % 1000}")
            .ToArray();
        File.WriteAllLines(inputPath, lines);

        // Act
        var sorter = new FileSorter(new FileSorterOptions { TempDirectory = _tempDirectory });
        await sorter.SortFile(inputPath, outputPath);

        // Assert
        var outputLines = await File.ReadAllLinesAsync(outputPath, TestContext.Current.CancellationToken);
        Assert.Equal(100000, outputLines.Length);
        
        // Verify sorting order
        for (var i = 1; i < outputLines.Length; i++)
        {
            var prevLine = outputLines[i - 1];
            var currentLine = outputLines[i];
            
            var prevParts = prevLine.Split(". ", 2);
            var currentParts = currentLine.Split(". ", 2);
            
            var prevText = prevParts[1];
            var currentText = currentParts[1];
            
            Assert.True(string.Compare(prevText, currentText, StringComparison.Ordinal) <= 0);
            
            if (prevText == currentText)
            {
                var prevNumber = int.Parse(prevParts[0]);
                var currentNumber = int.Parse(currentParts[0]);
                Assert.True(prevNumber <= currentNumber);
            }
        }
    }

    [Fact]
    public async Task SortFile_WithNonExistentInput_ThrowsException()
    {
        // Arrange
        var inputPath = Path.Combine(_testDirectory, "nonexistent.txt");
        var outputPath = Path.Combine(_testDirectory, "output.txt");

        // Act & Assert
        var sorter = new FileSorter(new FileSorterOptions { TempDirectory = _tempDirectory });
        await Assert.ThrowsAsync<FileNotFoundException>(() => sorter.SortFile(inputPath, outputPath));
    }

    [Fact]
    public async Task SortFile_WithInvalidOutputPath_ThrowsException()
    {
        // Arrange
        var inputPath = Path.Combine(_testDirectory, "input.txt");
        var outputPath = Path.Combine(_testDirectory, "invalid", "path", "output.txt");
        await File.WriteAllTextAsync(inputPath, "1. Test", TestContext.Current.CancellationToken);

        // Act & Assert
        var sorter = new FileSorter(new FileSorterOptions { TempDirectory = _tempDirectory });
        await Assert.ThrowsAsync<DirectoryNotFoundException>(() => sorter.SortFile(inputPath, outputPath));
    }
}