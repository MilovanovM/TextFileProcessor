using Xunit;
using TestFileGenerator.FileMergeStrategies;

namespace TestFileGenerator.Tests;

public class MergeStrategyTests : TestBase
{
    [Fact]
    public void StreamWriterMergeStrategy_MergesFilesCorrectly()
    {
        // Arrange
        var inputFiles = new List<string>
        {
            GetTempFilePath(),
            GetTempFilePath(),
            GetTempFilePath()
        };

        var expectedContent = new[]
        {
            "1. Line 1",
            "2. Line 2",
            "3. Line 3",
            "4. Line 4",
            "5. Line 5"
        };

        CreateTestFile(inputFiles[0], ["1. Line 1", "2. Line 2"]);
        CreateTestFile(inputFiles[1], ["3. Line 3"]);
        CreateTestFile(inputFiles[2], ["4. Line 4", "5. Line 5"]);

        var outputPath = GetTempFilePath();
        var strategy = new StreamWriterMergeStrategy();

        // Act
        var mergeTime = strategy.MergeFiles(inputFiles, outputPath);

        // Assert
        Assert.True(mergeTime > TimeSpan.Zero);
        var actualContent = File.ReadAllLines(outputPath);
        Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public void CatCommandMergeStrategy_MergesFilesCorrectly()
    {
        // Arrange
        var inputFiles = new List<string>
        {
            GetTempFilePath(),
            GetTempFilePath(),
            GetTempFilePath()
        };

        var expectedContent = new[]
        {
            "1. Line 1",
            "2. Line 2",
            "3. Line 3",
            "4. Line 4",
            "5. Line 5"
        };

        CreateTestFile(inputFiles[0], ["1. Line 1", "2. Line 2"]);
        CreateTestFile(inputFiles[1], ["3. Line 3"]);
        CreateTestFile(inputFiles[2], ["4. Line 4", "5. Line 5"]);

        var outputPath = GetTempFilePath();
        var strategy = new CatCommandMergeStrategy();

        // Act
        var mergeTime = strategy.MergeFiles(inputFiles, outputPath);

        // Assert
        Assert.True(mergeTime > TimeSpan.Zero);
        var actualContent = File.ReadAllLines(outputPath);
        Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public void FileMergeStrategyFactory_CreatesCorrectStrategy()
    {
        // Arrange & Act
        var streamWriterStrategy = FileMergeStrategyFactory.CreateStrategy(MergeStrategyNames.StreamWriter);
        var catStrategy = FileMergeStrategyFactory.CreateStrategy(MergeStrategyNames.Cat);
        var defaultStrategy = FileMergeStrategyFactory.CreateStrategy(null);

        // Assert
        Assert.IsType<StreamWriterMergeStrategy>(streamWriterStrategy);
        Assert.IsType<CatCommandMergeStrategy>(catStrategy);
        Assert.IsType<CatCommandMergeStrategy>(defaultStrategy);
    }

    [Fact]
    public void FileMergeStrategyFactory_ThrowsOnInvalidStrategy()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => FileMergeStrategyFactory.CreateStrategy("invalid"));
    }
} 