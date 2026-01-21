using AiFlow.Agents.DataAnalyzer.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AiFlow.Agents.DataAnalyzer.Agents;

public interface IDataAnalyzerAgent
{
    AIAgent Value { get; }
}

internal sealed class DataAnalyzerAgent : IDataAnalyzerAgent
{
    public AIAgent Value { get; }

    [Obsolete]
    public DataAnalyzerAgent(IChatClient chatClient, AnalysisTools tools)
    {
        var instructions =
            """
            You are a data analysis agent.

            Input: you will receive a ```csv_metadata``` block that contains csvPath.

            Always call AnalyzeCsv(csvPath) to get structured stats.

            Then create a human-friendly analysis JSON in a single code block:

            ```analysis
            {
              \"csvPath\": \"...\",
              \"scannedRows\": 123,
              \"numericColumns\": [...],
              \"keyFindings\": [\"...\"],
              \"dataQualityNotes\": [\"...\"],
              \"suggestedCharts\": [\"...\"],
              \"stats\": { ... },
              \"spikeDetection\": { ... }
            }
            ```

            Output rules:
            - Return ONLY the ```analysis``` code block.
            - Be concise: 5-10 key findings max.
            """;

        Value = new ChatClientAgent(
            chatClient,
            new ChatClientAgentOptions()
            {
                Name = "DataAnalyzer",
                ChatOptions = new ChatOptions
                {
                    Instructions = instructions,
                    Tools = [AIFunctionFactory.Create(tools.AnalyzeCsv)]
                }
            });
    }
}
