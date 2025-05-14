using System.Text;
using System.Diagnostics;
using TestFileGenerator.FileMergeStrategies;

namespace TestFileGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: TestFileGenerator <output_file_path> <file_size_in_mb> [merge_strategy]");
                Console.WriteLine($"Available merge strategies: {string.Join(", ", MergeStrategyNames.GetAll())}");
                return;
            }
            
            string? strategyName = null;
            var outputPath = args[0];

            if (args.Length > 2)
            {
                strategyName = args[2];
            }

            if (!int.TryParse(args[1], out var fileSizeMb) || fileSizeMb <= 0)
            {
                Console.WriteLine("Invalid file size. Please provide a valid number of megabytes.");
                return;
            }

            try
            {
                var stopwatch = Stopwatch.StartNew();
                GenerateTestFile(outputPath, fileSizeMb, strategyName);
                stopwatch.Stop();
                
                Console.WriteLine($"Test file generated successfully at: {outputPath}");
                Console.WriteLine($"Time taken: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating test file: {ex.Message}");
            }
        }

        public static void GenerateTestFile(string outputPath, int fileSizeMb, string? strategyName = null)
        {
            var targetSize = fileSizeMb * 1024L * 1024L; // Convert MB to bytes
            var numThreads = Environment.ProcessorCount;
            var chunkSize = targetSize / numThreads;
            var tempFiles = new List<string>();

            try
            {
                // Measure time for generating temporary files in parallel
                var chunkStopwatch = Stopwatch.StartNew();

                // Generate temporary files in parallel
                Parallel.For(0, numThreads, new ParallelOptions { MaxDegreeOfParallelism = numThreads },
                    threadIndex =>
                    {
                        var tempFile = Path.GetTempFileName();
                        tempFiles.Add(tempFile);
                        WriteRandomLinesToFile(tempFile, chunkSize);
                    });

                chunkStopwatch.Stop();
                Console.WriteLine(
                    $"Time to split files into chunks: {chunkStopwatch.Elapsed.TotalSeconds:F2} seconds");

                // Create and use the appropriate merge strategy
                var mergeStrategy = FileMergeStrategyFactory.CreateStrategy(strategyName);

                // Perform the merge and get the elapsed time
                var mergeTime = mergeStrategy.MergeFiles(tempFiles, outputPath);

                Console.WriteLine($"Time to merge chunks: {mergeTime.TotalSeconds:F2} seconds");
                Console.WriteLine($"Merge strategy used: {mergeStrategy.GetType().Name}");
            }
            finally
            {
                // Clean up temporary files
                foreach (var tempFile in tempFiles)
                {
                    try
                    {
                        if (File.Exists(tempFile))
                        {
                            File.Delete(tempFile);
                        }
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
            }
        }

        // Sample strings to ensure some duplicates
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

        private static void WriteRandomLinesToFile(string tempFile, long chunkSize)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            long currentSize = 0;

            using var writer = new StreamWriter(tempFile, false, Encoding.UTF8);
            while (currentSize < chunkSize)
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