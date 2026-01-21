namespace AiFlow.Core;

/// <summary>
/// Represents an outlier point detected during analysis.
/// </summary>
public sealed record OutlierPoint(
    /// <summary>
    /// The index of the row where the outlier was detected.
    /// </summary>
    int RowIndex,
    /// <summary>
    /// The value of the outlier point.
    /// </summary>
    double Value,
    /// <summary>
    /// The Z-score of the outlier point, indicating how many standard deviations it is from the mean.
    /// </summary>
    double ZScore);
