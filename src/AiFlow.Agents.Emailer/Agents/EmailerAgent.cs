using AiFlow.Agents.Emailer.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AiFlow.Agents.Emailer.Agents;

/// <summary>
/// Represents an agent that provides email writing and sending capabilities.
/// </summary>
public interface IEmailerAgent
{
    /// <summary>
    /// Gets the underlying AI agent instance.
    /// </summary>
    AIAgent Value { get; }
}

internal sealed class EmailerAgent : IEmailerAgent
{
    public AIAgent Value { get; }

    public EmailerAgent(IChatClient chatClient, EmailTools tools)
    {
        var instructions =
            """
            You are an email writing + sending agent.

            Input: you will receive a ```analysis``` block (JSON).

            Task:
            1) Draft a short, professional subject line.
            2) Draft a concise HTML email body with:
               - Title
               - Key findings (bullets)
               - Data quality notes (bullets)
               - Suggested charts (bullets)
            3) Call SendEmailAsync(subject, htmlBody, to?, dryRun?).

            If the user request includes an override recipient or dryRun flag, pass them to the tool.

            Output rules:
            - Return ONLY a ```email_result``` code block with the exact tool output.
            - No extra text.
            """;

        Value = new ChatClientAgent(
            chatClient,
            new ChatClientAgentOptions()
            {
                Name = "Emailer",
                ChatOptions = new ChatOptions
                {
                    Instructions = instructions,
                    Tools = [AIFunctionFactory.Create(tools.SendEmailAsync)]
                }
            });
    }
}
