namespace AiFlow.Core;

/// <summary>
/// Represents a spike alert detected in a data series, including the row index, score, and p-value.
/// </summary>
public sealed record SpikeAlert(
    /// <summary>
    /// The index of the row where the spike was detected.
    /// </summary>
    int RowIndex,
    /// <summary>
    /// The score indicating the strength of the spike.
    /// </summary>
    double Score,
    /// <summary>
    /// The p-value associated with the spike detection.
    /// </summary>
    double PValue);
