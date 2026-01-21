using AiFlow.Core.Abstractions;
using AiFlow.Core.Options;

namespace AiFlow.Core.Utilities;

/// <summary>
/// Provides file storage operations using the local file system.
/// </summary>
public sealed class LocalFileStore : IFileStore
{
    private readonly StorageOptions? _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalFileStore"/> class.
    /// </summary>
    /// <param name="options">The storage options.</param>
    // public LocalFileStore(IOptions<StorageOptions> options) // Fixes CS1591
    // {
    //   _options = options.Value;
    //  Directory.CreateDirectory(_options.RootPath);
    //}

    /// <inheritdoc/>
    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken ct)
    {
        var safeName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
        if (string.IsNullOrWhiteSpace(safeName)) safeName = "upload.csv";

        var path = Path.Combine(_options.RootPath, $"{Guid.NewGuid():N}_{safeName}");

        await using var fs = File.Create(path);
        await content.CopyToAsync(fs, ct);
        return path;
    }

    /// <inheritdoc/>
    public Task<Stream> OpenReadAsync(string path, CancellationToken ct)
    {
        Stream s = File.OpenRead(path);
        return Task.FromResult(s);
    }
}
