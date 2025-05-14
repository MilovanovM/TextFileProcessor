using System.Collections.Concurrent;

namespace FileSorter
{
    public readonly record struct LineItem(string Text, int Number);
    
    public class FileSorter(FileSorterOptions? options = null, ILineItemComparer? comparer = null)
    {
        private readonly FileSorterOptions _options = options ?? new FileSorterOptions();
        private readonly ILineItemComparer _comparer = comparer ?? new DefaultLineItemComparer();

        public async Task SortFile(string inputPath, string outputPath)
        {
            if (!Directory.Exists(_options.TempDirectory))
            {
                Directory.CreateDirectory(_options.TempDirectory);
            }

            try
            {
                // Step 1: Split the input file into sorted chunks
                var chunkFiles = await SplitIntoChunks(inputPath);

                // Step 2: Merge the sorted chunks
                await MergeChunks(chunkFiles, outputPath);
            }
            finally
            {
                if (Directory.Exists(_options.TempDirectory))
                {
                    Directory.Delete(_options.TempDirectory, true);
                }
            }
        }

        /// <summary>
        /// Splits the input file into smaller chunks, sorts each chunk, and writes the sorted chunks to temporary files.
        /// </summary>
        /// <param name="inputPath">The path of the input file to split into chunks.</param>
        /// <returns>A list of file paths to the sorted chunk files.</returns>
        private async Task<List<string>> SplitIntoChunks(string inputPath)
        {
            var chunkFiles = new ConcurrentBag<string>();
            var chunkIndex = 0;
            var semaphore = new SemaphoreSlim(Environment.ProcessorCount);
            var tasks = new List<Task>();

            using var reader = new StreamReader(File.OpenRead(inputPath), bufferSize: _options.BufferSize);
            var currentChunk = new List<LineItem>();

            while (await reader.ReadLineAsync() is { } line)
            {
                if (!TryParseLineItem(line, out var item)) continue;
            
                currentChunk.Add(item);

                if (currentChunk.Count < _options.ChunkSize) continue;

                var chunk = currentChunk;
                currentChunk = new List<LineItem>();
                var currentIndex = chunkIndex++;

                await semaphore.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await WriteSortedChunk(chunk, currentIndex, chunkFiles);
                        chunk.Clear();
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            if (currentChunk.Count > 0)
            {
                await WriteSortedChunk(currentChunk, chunkIndex, chunkFiles);
            }

            await Task.WhenAll(tasks);
            return chunkFiles.ToList();
        }

        /// <summary>
        /// Flush the given chunk to a temporary file and add it to the list of chunk files
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="chunkIndex"></param>
        /// <param name="chunkFiles"></param>
        private async Task WriteSortedChunk(List<LineItem> chunk, int chunkIndex, ConcurrentBag<string> chunkFiles)
        {
            var sortedChunk = chunk
                .OrderBy(x => x, _comparer)
                .ToList();

            var chunkPath = Path.Combine(_options.TempDirectory, $"chunk_{chunkIndex}.txt");
            await using var writer = new StreamWriter(File.Create(chunkPath), bufferSize: _options.BufferSize);
            
            foreach (var item in sortedChunk)
            {
                await writer.WriteLineAsync(FormatLineItem(item));
            }

            chunkFiles.Add(chunkPath);
            sortedChunk.Clear();
        }

        /// <summary>
        /// Use external merge strategy to merge sorted chunks into a single output file
        /// </summary>
        /// <param name="chunkFiles"></param>
        /// <param name="outputPath"></param>
        private async Task MergeChunks(List<string> chunkFiles, string outputPath)
        {
            var readers = new List<StreamReader>();
            var currentLines = new List<(LineItem Item, int ReaderIndex)>(chunkFiles.Count);

            try
            {
                // Open all chunk files
                foreach (var file in chunkFiles)
                {
                    readers.Add(new StreamReader(File.OpenRead(file), bufferSize: _options.BufferSize));
                }

                await using var writer = new StreamWriter(File.Create(outputPath), bufferSize: _options.BufferSize);
                
                // Initialize with the first line from each chunk
                for (var lineIndex = 0; lineIndex < readers.Count; lineIndex++)
                {
                    var line = await readers[lineIndex].ReadLineAsync();

                    if (line == null) continue;

                    if (TryParseLineItem(line, out var item))
                    {
                        currentLines.Add((item, lineIndex));
                    }
                }

                while (currentLines.Count > 0)
                {
                    // Find the minimum line based on sorting criteria
                    var minLine = currentLines
                        .OrderBy(x => x.Item, _comparer)
                        .First();

                    // Write the minimum line
                    await writer.WriteLineAsync(FormatLineItem(minLine.Item));

                    // Remove the written line and read the next line from that chunk
                    currentLines.Remove(minLine);
                    var nextLine = await readers[minLine.ReaderIndex].ReadLineAsync();
                    if (nextLine == null) continue;
                    
                    if (TryParseLineItem(nextLine, out var item))
                    {
                        currentLines.Add((item, minLine.ReaderIndex));
                    }
                }
            }
            finally
            {
                foreach (var reader in readers)
                {
                    reader.Dispose();
                }
            }
        }
        
        private const string Separator = ". ";

        private static bool TryParseLineItem(string line, out LineItem item)
        {
            item = default;
            
            if (string.IsNullOrWhiteSpace(line))
                return false;
        
            // guessing there is a more efficient way to do this, like IndexOf + Substring,
            // and it might be better on huge data (hundreds of gigabytes)
            var parts = line.Split(Separator, 2);
            if (parts.Length != 2 || !int.TryParse(parts[0], out var number))
                return false;
        
            item = new LineItem(parts[1], number);
            return true;
        }

        // guessing there is a more efficient way to do this, like StringBuilder,
        // and it might be better on huge data (hundreds of gigabytes)
        private static string FormatLineItem(LineItem item) => $"{item.Number}{Separator}{item.Text}";
    }
}
