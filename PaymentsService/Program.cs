using PaymentsService.Data;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using PaymentsService.Messaging;

var factory = new ConnectionFactory
{
    HostName = "rabbitmq"
};

IConnection? connection = null;
for (int i = 0; i < 1000; i++)
{
    try
    {
        connection = factory.CreateConnection();
        break;
    }
    catch
    {
        Console.WriteLine("RabbitMQ not ready, retrying...");
        Thread.Sleep(1000);
    }
}
if (connection == null) throw new Exception("Cannot connect to RabbitMQ");

var channel = connection.CreateModel();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton(channel);
builder.Services.AddHostedService<PaymentsHandler>();

var connectionString = builder.Configuration["DB_CONNECTION"]
    ?? throw new InvalidOperationException("DB_CONNECTION not set");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.EnsureCreated();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
