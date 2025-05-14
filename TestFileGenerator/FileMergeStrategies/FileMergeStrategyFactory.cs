using System.Runtime.InteropServices;

namespace TestFileGenerator.FileMergeStrategies
{
    /// <summary>
    /// Factory for creating file merge strategies based on the current operating system
    /// </summary>
    public static class FileMergeStrategyFactory
    {
        /// <summary>
        /// Creates a suitable file merge strategy for the current environment
        /// </summary>
        /// <param name="strategyName">Optional strategy name to override automatic selection</param>
        /// <returns>An implementation of IFileMergeStrategy</returns>
        public static IFileMergeStrategy CreateStrategy(string? strategyName = null)
        {
            // If a specific strategy is requested, use that
            if (!string.IsNullOrEmpty(strategyName))
            {
                return strategyName.ToLowerInvariant() switch
                {
                    MergeStrategyNames.Cat => new CatCommandMergeStrategy(),
                    MergeStrategyNames.StreamWriter => new StreamWriterMergeStrategy(),
                    _ => throw new ArgumentException($"Unknown merge strategy: {strategyName}. " +
                         $"Available strategies: {string.Join(", ", MergeStrategyNames.GetAll())}")
                };
            }
            
            // Otherwise, choose based on the operating system
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new CatCommandMergeStrategy();
            }
            
            // Default to StreamWriter for all other platforms
            return new StreamWriterMergeStrategy();
        }
    }
}
