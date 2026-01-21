namespace AiFlow.Core;

/// <summary>
/// Represents the result of a data analysis report for a CSV file, including scanned rows, numeric columns, statistics, spike detection results, key findings, data quality notes, and suggested charts.
/// </summary>
public sealed record AnalysisReport(
    /// <summary>
    /// The file path to the CSV file that was analyzed.
    /// </summary>
    string CsvPath,
    /// <summary>
    /// The number of rows scanned in the analysis.
    /// </summary>
    int ScannedRows,
    /// <summary>
    /// The list of numeric columns found in the CSV file.
    /// </summary>
    IReadOnlyList<string> NumericColumns,
    /// <summary>
    /// The statistics for each numeric column.
    /// </summary>
    IReadOnlyDictionary<string, ColumnStats> Stats,
    /// <summary>
    /// The spike detection results for a specific column, if any.
    /// </summary>
    SpikeDetection? SpikeDetection,
    /// <summary>
    /// The key findings from the analysis.
    /// </summary>
    IReadOnlyList<string> KeyFindings,
    /// <summary>
    /// Notes about data quality issues found during analysis.
    /// </summary>
    IReadOnlyList<string> DataQualityNotes,
    /// <summary>
    /// Suggested charts to visualize the data.
    /// </summary>
    IReadOnlyList<string> SuggestedCharts);
