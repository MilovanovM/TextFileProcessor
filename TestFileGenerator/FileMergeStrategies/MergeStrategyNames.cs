namespace TestFileGenerator.FileMergeStrategies
{
    /// <summary>
    /// Constants for available merge strategy names
    /// </summary>
    public static class MergeStrategyNames
    {
        /// <summary>
        /// Strategy that uses the 'cat' command on macOS
        /// </summary>
        public const string Cat = "cat";
        
        /// <summary>
        /// Strategy that uses .NET's StreamWriter for merging
        /// </summary>
        public const string StreamWriter = "streamwriter";
        
        /// <summary>
        /// Gets all available strategy names for display in help text
        /// </summary>
        /// <returns>Array of strategy names</returns>
        public static string[] GetAll() => [Cat, StreamWriter];
    }
}
