namespace TestFileGenerator.Simple.Tests;

public class CommandLineTests
{
    [Theory]
    [InlineData("", "Usage: TestFileGenerator.Simple <output_file_path> <file_size_in_mb>")]
    [InlineData("output.txt", "Usage: TestFileGenerator.Simple <output_file_path> <file_size_in_mb>")]
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