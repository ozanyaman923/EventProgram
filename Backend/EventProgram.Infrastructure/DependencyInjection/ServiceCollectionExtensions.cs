using EventProgram.Application.Abstractions.Events;
using EventProgram.Infrastructure.Persistence;
using EventProgram.Infrastructure.Services.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventProgram.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<EventProgramDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IEventReadService, EventReadService>();
        return services;
    }
}
