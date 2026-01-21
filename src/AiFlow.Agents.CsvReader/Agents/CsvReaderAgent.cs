using AiFlow.Agents.CsvReader.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AiFlow.Agents.CsvReader.Agents;

/// <summary>
/// Represents an agent that provides access to a CSV reader AI agent.
/// </summary>
public interface ICsvReaderAgent
{
    /// <summary>
    /// Gets the underlying AI agent instance.
    /// </summary>
    AIAgent Value { get; }
}

internal sealed class CsvReaderAgent : ICsvReaderAgent
{
    public AIAgent Value { get; }

    public CsvReaderAgent(IChatClient chatClient, CsvTools tools)
    {
        var instructions =
            """
            You are a CSV reader agent.

            Always call ReadCsvMetadata(csvPath, previewRows) to fetch structured CSV metadata.

            Output rules:
            - Return ONLY a ```csv_metadata``` code block.
            - The code block MUST contain the exact tool output (JSON).
            - No extra text.
            """;

        Value = new ChatClientAgent(
                    chatClient,
                    new ChatClientAgentOptions
                    {
                        Name = "CsvReader",
                        ChatOptions = new ChatOptions
                        {
                            Instructions = instructions,
                            Tools = [AIFunctionFactory.Create(tools.ReadCsvMetadata)]
                        }
                    });
    }
}
