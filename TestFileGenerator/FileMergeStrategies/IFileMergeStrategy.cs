using System.Diagnostics;

namespace TestFileGenerator.FileMergeStrategies
{
    /// <summary>
    /// Interface defining a strategy for merging multiple files into a single output file
    /// </summary>
    public interface IFileMergeStrategy
    {
        /// <summary>
        /// Merges the given temporary files into a single output file
        /// </summary>
        /// <param name="tempFiles">List of temporary file paths to merge</param>
        /// <param name="outputPath">Path for the merged output file</param>
        /// <returns>Elapsed time for the merge operation</returns>
        TimeSpan MergeFiles(List<string> tempFiles, string outputPath);
    }
}
