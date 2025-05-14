using System.Diagnostics;
using System.Text;

namespace TestFileGenerator.FileMergeStrategies
{
    /// <summary>
    /// Strategy for merging files using .NET's StreamWriter and StreamReader
    /// </summary>
    public class StreamWriterMergeStrategy : IFileMergeStrategy
    {
        public TimeSpan MergeFiles(List<string> tempFiles, string outputPath)
        {
            var mergeStopwatch = Stopwatch.StartNew();
            
            // Merge temporary files using StreamWriter
            using (var outputWriter = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                foreach (var tempFile in tempFiles)
                {
                    using var reader = new StreamReader(tempFile);
                    while (reader.ReadLine() is { } line)
                    {
                        outputWriter.WriteLine(line);
                    }
                }
            }
            
            mergeStopwatch.Stop();
            return mergeStopwatch.Elapsed;
        }
    }
}
