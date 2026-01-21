using AiFlow.Core;
using CsvHelper.Configuration;
using System.ComponentModel;
using System.Globalization;

namespace AiFlow.Agents.CsvReader.Tools;

/// <summary>
/// Provides utility methods for reading CSV files, including metadata extraction and preview functionality.
/// </summary>
public sealed class CsvTools
{
    /// <summary>
    /// Reads a CSV file and returns metadata and a small preview (as a structured JSON object).
    /// </summary>
    /// <param name="csvPath">Path to CSV file</param>
    /// <param name="previewRows">How many rows to preview</param>
    /// <returns>Metadata and preview of the CSV file</returns>
    public CsvMetadata ReadCsvMetadata(
        [Description("Path to CSV file")] string csvPath,
        [Description("How many rows to preview")] int previewRows = 20)
    {
        if (!File.Exists(csvPath)) throw new FileNotFoundException("CSV not found", csvPath);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            DetectDelimiter = true,
            BadDataFound = null,
            MissingFieldFound = null,
            HeaderValidated = null
        };

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvHelper.CsvReader(reader, config); // Fully qualify CsvReader

        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord ?? Array.Empty<string>();

        var preview = new List<IReadOnlyDictionary<string, string?>>();
        int count = 0;
        while (count < previewRows && csv.Read())
        {
            var row = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var h in headers)
                row[h] = csv.GetField(h);

            preview.Add(row);
            count++;
        }

        var rowCount = File.ReadLines(csvPath).Skip(1).Count();
        return new CsvMetadata(csvPath, rowCount, headers, preview);
    }
}
