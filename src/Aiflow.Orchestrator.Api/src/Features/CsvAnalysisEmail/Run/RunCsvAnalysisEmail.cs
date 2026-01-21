using System.Text.Json;
using AiFlow.Core;
using AiFlow.Core.Abstractions;
using AiFlow.Orchestrator.Api.Workflow;
using Microsoft.AspNetCore.Mvc;

namespace AiFlow.Orchestrator.Api.Features.CsvAnalysisEmail.Run;

public static class RunCsvAnalysisEmail
{
    public static IEndpointRouteBuilder MapRunCsvAnalysisEmail(this IEndpointRouteBuilder app)
    {
        app.MapPost("/workflows/csv-analysis-email", HandleAsync)
            .WithName("RunCsvAnalysisEmail")
            .Produces<RunResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    public sealed class RunRequest
    {
        [FromForm(Name = "csv")]
        public IFormFile? CsvFile { get; set; }

        [FromForm(Name = "to")]
        public string? ToOverride { get; set; }

        [FromForm(Name = "dryRun")]
        public bool? DryRunOverride { get; set; }

        [FromForm(Name = "spikeColumn")]
        public string? SpikeColumn { get; set; }

        [FromForm(Name = "previewRows")]
        public int PreviewRows { get; set; } = 20;
    }

    public sealed record RunResponse(
        string CsvPath,
        CsvMetadata? CsvMetadata,
        JsonElement? Analysis,
        EmailSendResult? EmailResult,
        string RawText);

    private static async Task<IResult> HandleAsync(
        [FromServices] IFileStore fileStore,
        [FromServices] ICsvAnalysisEmailWorkflow workflow,
        [FromForm] RunRequest request,
        CancellationToken ct)
    {
        if (request.CsvFile is null || request.CsvFile.Length == 0)
            return Results.BadRequest(new { error = "Missing csv form file (field name: csv)" });

        var csvPath = await fileStore.SaveAsync(request.CsvFile.OpenReadStream(), request.CsvFile.FileName, ct);

        var prompt = $$"""
Read the CSV and send an analysis by email.

Inputs:
- csvPath: {{csvPath}}
- previewRows: {{request.PreviewRows}}
- spikeColumn (optional): {{request.SpikeColumn ?? ""}}
- toOverride (optional): {{request.ToOverride ?? ""}}
- dryRunOverride (optional): {{request.DryRunOverride?.ToString() ?? ""}}

Rules:
- CsvReader must return ```csv_metadata```
- DataAnalyzer must return ```analysis```
- Emailer must return ```email_result```
""";

        var workflowAgent = await workflow.GetAgentAsync();
        var response = await workflowAgent.RunAsync(prompt);

        var raw = response.Text ?? string.Empty;

        CsvMetadata? metadata = null;
        JsonElement? analysis = null;
        EmailSendResult? emailResult = null;

        try
        {
            var metaText = CodeBlocks.Extract(raw, "csv_metadata");
            if (!string.IsNullOrWhiteSpace(metaText))
                metadata = JsonSerializer.Deserialize<CsvMetadata>(metaText);

            var analysisText = CodeBlocks.Extract(raw, "analysis");
            if (!string.IsNullOrWhiteSpace(analysisText))
            {
                using var doc = JsonDocument.Parse(analysisText);
                analysis = doc.RootElement.Clone();
            }

            var emailText = CodeBlocks.Extract(raw, "email_result");
            if (!string.IsNullOrWhiteSpace(emailText))
                emailResult = JsonSerializer.Deserialize<EmailSendResult>(emailText);
        }
        catch
        {
            // If the model deviates from the expected schema, we still return raw text.
        }

        return Results.Ok(new RunResponse(csvPath, metadata, analysis, emailResult, raw));
    }
}
