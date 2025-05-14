using Xunit;

namespace TestFileGenerator.Tests;

public class CommandLineTests
{
    [Theory]
    [InlineData("", "Usage: TestFileGenerator <output_file_path> <file_size_in_mb> [merge_strategy]")]
    [InlineData("output.txt", "Usage: TestFileGenerator <output_file_path> <file_size_in_mb> [merge_strategy]")]
    public void Main_WithInvalidArguments_ShowsUsageMessage(string args, string expectedMessage)
    {
        // Arrange
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        Program.Main(args.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains(expectedMessage, output);
    }

    [Theory]
    [InlineData("output.txt invalid", "Invalid file size")]
    [InlineData("output.txt -1", "Invalid file size")]
    public void Main_WithInvalidFileSize_ShowsErrorMessage(string args, string expectedMessage)
    {
        // Arrange
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        Program.Main(args.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains(expectedMessage, output);
    }

    [Theory]
    [InlineData("output.txt 1")]
    [InlineData("output.txt 1 streamwriter")]
    [InlineData("output.txt 1 cat")]
    public void Main_WithValidArguments_DoesNotShowError(string args)
    {
        // Arrange
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        Program.Main(args.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        // Assert
        var output = stringWriter.ToString();
        Assert.DoesNotContain("Error", output);
        Assert.DoesNotContain("Usage", output);
    }
} 