
using EventProgram.Infrastructure.DependencyInjection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("EventProgramDatabase")
    ?? throw new InvalidOperationException(
        "The EventProgramDatabase connection string is not configured.");

builder.Services.AddInfrastructure(connectionString);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization(); 
app.MapControllers();

app.Run();

        
