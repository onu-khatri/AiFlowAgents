using AiFlow.Agents.Emailer.Agents;
using AiFlow.Agents.Emailer.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace AiFlow.Agents.Emailer;

/// <summary>
/// Extension methods for registering the Emailer agent and related services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Emailer agent and its dependencies to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add the agent to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddEmailerAgent(this IServiceCollection services)
    {
        services.AddSingleton<EmailTools>();
        services.AddSingleton<IEmailerAgent, EmailerAgent>();
        return services;
    }
}
