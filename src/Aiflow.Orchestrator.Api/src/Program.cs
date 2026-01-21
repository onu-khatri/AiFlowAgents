using AiFlow.Agents.CsvReader;
using AiFlow.Agents.DataAnalyzer;
using AiFlow.Agents.Emailer;
using AiFlow.Core;
using AiFlow.Orchestrator.Api.Features.CsvAnalysisEmail.Run;
using AiFlow.Orchestrator.Api.Workflow;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCsvEmailCore(builder.Configuration);

// IChatClient for Agent Framework (via Microsoft.Extensions.AI.OpenAI)
// Uses the official OpenAI .NET client under the hood.
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var opts = sp.GetRequiredService<IOptions<OpenAiOptions>>().Value;
    if (string.IsNullOrWhiteSpace(opts.ApiKey))
        throw new InvalidOperationException("OpenAI:ApiKey is not configured.");

    // OpenAI.Chat.ChatClient (OpenAI NuGet package)
    var chat = new ChatClient(opts.Model, new ApiKeyCredential(opts.ApiKey));
    return chat.AsIChatClient();
});

// Agents (separate projects)
builder.Services.AddCsvReaderAgent();
builder.Services.AddDataAnalyzerAgent();
builder.Services.AddEmailerAgent();

// Workflow (CsvReader -> DataAnalyzer -> Emailer)
builder.Services.AddSingleton<ICsvAnalysisEmailWorkflow, CsvAnalysisEmailWorkflow>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapRunCsvAnalysisEmail();

app.Run();
