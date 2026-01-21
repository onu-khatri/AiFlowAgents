using AiFlow.Core;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.ComponentModel;
using System.Globalization;

namespace AiFlow.Agents.DataAnalyzer.Tools;

/// <summary>
/// Provides tools for analyzing CSV files, including numeric column statistics,
/// outlier detection, and ML.NET spike detection for time series data.
/// </summary>
public sealed class AnalysisTools
{
    private readonly MLContext _ml = new(seed: 42);

    private sealed class ValueRow { public float Value { get; set; } }
    private sealed class SpikePrediction
    {
        [VectorType(3)]
        public double[] Prediction { get; set; } = Array.Empty<double>();
    }

    /// <summary>
    /// Analyzes a CSV file to compute statistics for numeric columns, detect outliers, and perform ML.NET spike detection on a selected numeric column.
    /// Returns an <see cref="AnalysisReport"/> containing structured results including column statistics, outlier samples, spike detection alerts, and suggestions for further analysis.
    /// </summary>
    /// <param name="csvPath">Path to the CSV file to analyze.</param>
    /// <param name="spikeColumn">Optional: Name of the numeric column to use for spike detection. If not specified, the first numeric column is used.</param>
    /// <param name="maxRows">Maximum number of rows to scan for numeric statistics. Default is 5000.</param>
    /// <returns>An <see cref="AnalysisReport"/> with analysis results.</returns>
    [Description("Analyze a CSV file: numeric column stats + outliers + ML.NET spike detection for a numeric column. Returns structured JSON.")]
    [Obsolete]
    public AnalysisReport AnalyzeCsv(
        [Description("Path to CSV file")] string csvPath,
        [Description("Optional: which numeric column to use for spike detection")] string? spikeColumn = null,
        [Description("Max rows to scan for numeric stats")] int maxRows = 5000)
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
        using var csv = new CsvReader(reader, config);

        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord ?? Array.Empty<string>();

        var numeric = headers.ToDictionary(h => h, _ => new List<double>(), StringComparer.OrdinalIgnoreCase);

        int rows = 0;
        while (rows < maxRows && csv.Read())
        {
            foreach (var h in headers)
            {
                var s = csv.GetField(h);
                if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                    numeric[h].Add(d);
            }
            rows++;
        }

        var numericCols = numeric
            .Where(kvp => kvp.Value.Count >= Math.Max(20, rows / 10))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

        var stats = new Dictionary<string, ColumnStats>(StringComparer.OrdinalIgnoreCase);

        foreach (var (col, values) in numericCols)
        {
            var n = values.Count;
            var mean = values.Average();
            var variance = values.Sum(v => (v - mean) * (v - mean)) / Math.Max(1, n - 1);
            var std = Math.Sqrt(variance);

            var outliers = values
                .Select((v, idx) => new { v, idx, z = std == 0 ? 0 : (v - mean) / std })
                .Where(x => Math.Abs(x.z) >= 3)
                .Take(10)
                .Select(x => new OutlierPoint(
                    RowIndex: x.idx + 2,
                    Value: x.v,
                    ZScore: Math.Round(x.z, 3)))
                .ToList();

            stats[col] = new ColumnStats(
                Count: n,
                Min: values.Min(),
                Max: values.Max(),
                Mean: Math.Round(mean, 4),
                Std: Math.Round(std, 4),
                SampleOutliers: outliers);
        }

        SpikeDetection? spikes = null;
        if (numericCols.Count > 0)
        {
            var chosen = (!string.IsNullOrWhiteSpace(spikeColumn) && numericCols.ContainsKey(spikeColumn))
                ? spikeColumn!
                : numericCols.Keys.First();

            var series = numericCols[chosen];
            if (series.Count >= 20)
            {
                var data = _ml.Data.LoadFromEnumerable(series.Select(v => new ValueRow { Value = (float)v }));
                var pipeline = _ml.Transforms.DetectIidSpike(
                    outputColumnName: "Prediction",
                    inputColumnName: nameof(ValueRow.Value),
                    confidence: 95,
                    pvalueHistoryLength: Math.Min(100, series.Count));

                var model = pipeline.Fit(data);
                var transformed = model.Transform(data);
                var preds = _ml.Data.CreateEnumerable<SpikePrediction>(transformed, reuseRowObject: false).ToList();

                var alerts = preds
                    .Select((p, i) => new
                    {
                        i,
                        alert = p.Prediction.ElementAtOrDefault(0),
                        score = p.Prediction.ElementAtOrDefault(1),
                        pvalue = p.Prediction.ElementAtOrDefault(2)
                    })
                    .Where(x => x.alert >= 1)
                    .Take(10)
                    .Select(x => new SpikeAlert(
                        RowIndex: x.i + 2,
                        Score: Math.Round(x.score, 4),
                        PValue: Math.Round(x.pvalue, 6)))
                    .ToList();

                spikes = new SpikeDetection(chosen, alerts);
            }
        }

        // The LLM will write the human-friendly findings.
        return new AnalysisReport(
            CsvPath: csvPath,
            ScannedRows: rows,
            NumericColumns: numericCols.Keys.ToList(),
            Stats: stats,
            SpikeDetection: spikes,
            KeyFindings: Array.Empty<string>(),
            DataQualityNotes: Array.Empty<string>(),
            SuggestedCharts: Array.Empty<string>());
    }
}
