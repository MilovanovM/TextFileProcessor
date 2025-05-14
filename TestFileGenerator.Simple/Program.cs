using System.Text;
using System.Diagnostics;

namespace TestFileGenerator.Simple
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: TestFileGenerator.Simple <output_file_path> <file_size_in_mb>");
                return;
            }

            var outputPath = args[0];

            if (!int.TryParse(args[1], out var fileSizeMb) || fileSizeMb <= 0)
            {
                Console.WriteLine("Invalid file size. Please provide a valid number of megabytes.");
                return;
            }

            try
            {
                var stopwatch = Stopwatch.StartNew();
                WriteRandomLinesToFile(outputPath, fileSizeMb * 1024L * 1024L);
                stopwatch.Stop();
                
                Console.WriteLine($"Test file generated successfully at: {outputPath}");
                Console.WriteLine($"Time taken: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating test file: {ex.Message}");
            }
        }

        // Sample strings
        private static readonly string[] SampleStrings =
        [
            "Apple",
            "Banana is yellow",
            "Cherry is the best",
            "Something something something",
            "Unique string 1",
            "Unique string 2",
            "Unique string 3"
        ];

        public static void WriteRandomLinesToFile(string tempFile, long fileSizeBytes)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            long currentSize = 0;

            using var writer = new StreamWriter(tempFile, false, Encoding.UTF8);
            while (currentSize < fileSizeBytes)
            {
                var number = random.Next(1, 100000);
                var text = SampleStrings[random.Next(SampleStrings.Length)];
                var line = $"{number}. {text}";
                writer.WriteLine(line);
                currentSize += Encoding.UTF8.GetByteCount(line + Environment.NewLine);
            }
        }
    }
}