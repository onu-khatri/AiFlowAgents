namespace AiFlow.Core.Abstractions;

/// <summary>
/// Provides methods for saving and reading files in a storage system.
/// </summary>
public interface IFileStore
{
    /// <summary>
    /// Saves the specified content stream to a file with the given name.
    /// </summary>
    /// <param name="content">The stream containing the file content.</param>
    /// <param name="fileName">The name of the file to save.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The path to the saved file.</returns>
    Task<string> SaveAsync(Stream content, string fileName, CancellationToken ct);

    /// <summary>
    /// Opens a stream for reading the file at the specified path.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A stream for reading the file.</returns>
    Task<Stream> OpenReadAsync(string path, CancellationToken ct);
}
