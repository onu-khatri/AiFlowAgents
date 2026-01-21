using AiFlow.Agents.DataAnalyzer.Agents;
using AiFlow.Agents.DataAnalyzer.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace AiFlow.Agents.DataAnalyzer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAnalyzerAgent(this IServiceCollection services)
    {
        services.AddSingleton<AnalysisTools>();
        services.AddSingleton<IDataAnalyzerAgent, DataAnalyzerAgent>();
        return services;
    }
}
