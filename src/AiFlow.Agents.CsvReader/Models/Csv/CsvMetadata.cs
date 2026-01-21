namespace AiFlow.Core;

/// <summary>
/// Metadata information about a CSV file, including its path, row count, column names, and a preview of its rows.
/// </summary>
public sealed record CsvMetadata(
    /// <summary>
    /// The file path to the CSV file.
    /// </summary>
    string CsvPath,
    /// <summary>
    /// The total number of rows in the CSV file.
    /// </summary>
    int RowCount,
    /// <summary>
    /// The list of column names in the CSV file.
    /// </summary>
    IReadOnlyList<string> Columns,
    /// <summary>
    /// A preview of the rows in the CSV file, where each row is represented as a dictionary mapping column names to values.
    /// </summary>
    IReadOnlyList<IReadOnlyDictionary<string, string?>> PreviewRows);
