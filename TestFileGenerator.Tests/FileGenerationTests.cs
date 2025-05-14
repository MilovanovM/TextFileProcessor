using Xunit;
using TestFileGenerator.FileMergeStrategies;

namespace TestFileGenerator.Tests;

public class FileGenerationTests : TestBase
{
    [Fact]
    public void GenerateTestFile_CreatesFileWithCorrectSize()
    {
        // Arrange
        var outputPath = GetTempFilePath();
        const int fileSizeMb = 1;

        // Act
        Program.GenerateTestFile(outputPath, fileSizeMb);

        // Assert
        var fileInfo = new FileInfo(outputPath);
        Assert.True(fileInfo.Exists);
        Assert.True(fileInfo.Length >= fileSizeMb * 1024L * 1024L);
    }

    [Theory]
    [InlineData(MergeStrategyNames.StreamWriter)]
    [InlineData(MergeStrategyNames.Cat)]
    public void GenerateTestFile_WithDifferentMergeStrategies_CreatesValidFile(string strategyName)
    {
        // Arrange
        var outputPath = GetTempFilePath();
        const int fileSizeMb = 1;

        // Act
        Program.GenerateTestFile(outputPath, fileSizeMb, strategyName);

        // Assert
        var fileInfo = new FileInfo(outputPath);
        Assert.True(fileInfo.Exists);
        Assert.True(fileInfo.Length >= fileSizeMb * 1024L * 1024L);
        
        var lines = File.ReadAllLines(outputPath);
        Assert.NotEmpty(lines);
    }
} 