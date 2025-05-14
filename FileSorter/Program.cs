using System.Diagnostics;
namespace FileSorter;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: FileSorter <input_file_path> <output_file_path>");
            return;
        }

        var inputPath = args[0];
        var outputPath = args[1];

        try
        {
            var stopwatch = Stopwatch.StartNew();
            await new FileSorter().SortFile(inputPath, outputPath);
            stopwatch.Stop();

            Console.WriteLine("File sorted successfully!");
            Console.WriteLine($"Time spent: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sorting file: {ex.Message}");
        }
    }
}
