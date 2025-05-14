using System.Diagnostics;

namespace TestFileGenerator.FileMergeStrategies
{
    /// <summary>
    /// Strategy for merging files using the 'cat' command on macOS
    /// </summary>
    public class CatCommandMergeStrategy : IFileMergeStrategy
    {
        public TimeSpan MergeFiles(List<string> tempFiles, string outputPath)
        {
            var mergeStopwatch = Stopwatch.StartNew();
            
            // Create a bash command to run cat and redirect output to the file
            var catArguments = string.Join(" ", tempFiles.Select(file => $"\"{file}\""));
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"cat {catArguments} > \\\"{outputPath}\\\"\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = false,
                RedirectStandardError = true
            };
            
            // Start the process and wait for it to complete
            using (var process = Process.Start(processStartInfo))
            {
                if (process == null)
                {
                    throw new Exception("Failed to start bash process");
                }
                process.WaitForExit();
                
                // Check if the process completed successfully
                if (process.ExitCode != 0)
                {
                    throw new Exception($"Cat command failed with exit code {process.ExitCode}");
                }
            }
            
            mergeStopwatch.Stop();
            return mergeStopwatch.Elapsed;
        }
    }
}
