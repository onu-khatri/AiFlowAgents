using AiFlow.Agents.CsvReader.Agents;
using AiFlow.Agents.CsvReader.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace AiFlow.Agents.CsvReader;

/// <summary>
/// Extension methods for registering CSV reader agent services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the CSV reader agent and related tools as singleton services.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCsvReaderAgent(this IServiceCollection services)
    {
        services.AddSingleton<CsvTools>();
        services.AddSingleton<ICsvReaderAgent, CsvReaderAgent>();
        return services;
    }
}
