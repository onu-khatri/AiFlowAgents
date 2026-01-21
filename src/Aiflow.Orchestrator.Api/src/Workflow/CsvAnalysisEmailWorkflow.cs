using AiFlow.Agents.CsvReader.Agents;
using AiFlow.Agents.DataAnalyzer.Agents;
using AiFlow.Agents.Emailer.Agents;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

namespace AiFlow.Orchestrator.Api.Workflow;

public interface ICsvAnalysisEmailWorkflow
{
    Task<AIAgent> GetAgentAsync();
}

internal sealed class CsvAnalysisEmailWorkflow : ICsvAnalysisEmailWorkflow
{
    private readonly ICsvReaderAgent _csvReader;
    private readonly IDataAnalyzerAgent _analyzer;
    private readonly IEmailerAgent _emailer;

    private readonly SemaphoreSlim _gate = new(1, 1);
    private AIAgent? _cached;

    public CsvAnalysisEmailWorkflow(ICsvReaderAgent csvReader, IDataAnalyzerAgent analyzer, IEmailerAgent emailer)
    {
        _csvReader = csvReader;
        _analyzer = analyzer;
        _emailer = emailer;
    }

    public async Task<AIAgent> GetAgentAsync()
    {
        if (_cached is not null) return _cached;

        await _gate.WaitAsync();
        try
        {
            if (_cached is not null) return _cached;

            // Pipeline: CsvReader -> DataAnalyzer -> Emailer
            var agents = new[] { _csvReader.Value, _analyzer.Value, _emailer.Value };
            var workflow = AgentWorkflowBuilder.BuildSequential(agents);

            _cached = workflow.AsAgent(
                id: "csv-email-workflow",
                name: "CSV Email Workflow",
                description: "CsvReader -> Analyzer -> Emailer"
            );

            return _cached;
        }
        finally
        {
            _gate.Release();
        }
    }
}
