
namespace ApiGateway;

public class Program
{
    private static void AddNamedHttpClientFromEnv(WebApplicationBuilder builder, string clientName)
    {
        string envName = clientName.ToUpper();
        var baseUrl = builder.Configuration[envName];
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException($"Environment variable '{envName}' is not set");
        }
        builder.Services.AddHttpClient(clientName, client => { client.BaseAddress = new Uri(baseUrl); });
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        AddNamedHttpClientFromEnv(builder, "Payments");
        AddNamedHttpClientFromEnv(builder, "Orders");

        var app = builder.Build();

        app.UseCors();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();

    }
}
