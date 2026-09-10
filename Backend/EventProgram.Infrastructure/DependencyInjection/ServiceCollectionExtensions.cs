using EventProgram.Application.Abstractions.Events;
using EventProgram.Application.Abstractions.Users;
using EventProgram.Infrastructure.Persistence;
using EventProgram.Infrastructure.Services.Events;
using EventProgram.Infrastructure.Services.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventProgram.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<EventProgramDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IEventReadService, EventReadManager>();
        services.AddScoped<IEventWriteService, EventWriteManager>();
        services.AddScoped<IUserAccountService, UserAccountManager>();
        return services;
    }
}
