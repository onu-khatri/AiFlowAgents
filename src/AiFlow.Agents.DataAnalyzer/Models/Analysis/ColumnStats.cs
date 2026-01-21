namespace AiFlow.Core;

/// <summary>
/// Statistical summary for a numeric column, including count, min, max, mean, standard deviation, and sample outliers.
/// </summary>
/// <param name="Count">The number of non-null values in the column.</param>
/// <param name="Min">The minimum value in the column.</param>
/// <param name="Max">The maximum value in the column.</param>
/// <param name="Mean">The mean (average) value of the column.</param>
/// <param name="Std">The standard deviation of the column values.</param>
/// <param name="SampleOutliers">A sample of outlier points detected in the column.</param>
public sealed record ColumnStats(
    int Count,
    double Min,
    double Max,
    double Mean,
    double Std,
    IReadOnlyList<OutlierPoint> SampleOutliers);
