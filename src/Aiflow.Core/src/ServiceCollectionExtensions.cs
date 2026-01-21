using AiFlow.Core.Abstractions;
using AiFlow.Core.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiFlow.Core;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core services for CSV Email functionality, including configuration for OpenAI, SMTP, and Storage options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCsvEmailCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenAiOptions>(configuration.GetSection(OpenAiOptions.SectionName));
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));

        services.AddSingleton<IFileStore, LocalFileStore>();
        return services;
    }
}
