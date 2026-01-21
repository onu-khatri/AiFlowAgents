namespace AiFlow.Core;

/// <summary>
/// Represents the result of spike detection analysis for a specific column, including the column name and a list of top spike alerts.
/// </summary>
public sealed record SpikeDetection(
    /// <summary>
    /// The name of the column where spike detection was performed.
    /// </summary>
    string Column,
    /// <summary>
    /// The list of top spike alerts detected in the column.
    /// </summary>
    IReadOnlyList<SpikeAlert> TopAlerts);
