namespace FileSorter;

public class FileSorterOptions
{
    /// <summary>
    /// Chunk size, default is 10M lines
    /// </summary>
    public int ChunkSize { get; init; } = 10 * 1024 * 1024;

    /// <summary>
    /// StreamReader/StreamWriter buffer, default is 1MB
    /// </summary>
    public int BufferSize { get; init; } = 1024 * 1024;

    public string TempDirectory { get; init; } = Path.Combine(Path.GetTempPath(), "FileSorterTemp");
}
