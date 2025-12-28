using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using PaymentsService.Data;
namespace PaymentsService.Messaging;

public class PaymentsHandler : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IModel channel;

    public PaymentsHandler(IServiceScopeFactory scopeFactory, IModel channel)
    {
        this.scopeFactory = scopeFactory;
        this.channel = channel;

        this.channel.QueueDeclare(
            queue: "payment_requests",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        this.channel.QueueDeclare(
            queue: "payment_results",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (sender, ea) =>
        {
            var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
            var parts = msg.Split('|');

            var orderId = parts[0];
            var userId = parts[1];
            var amount = decimal.Parse(parts[2]);

            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var session = db.Database.BeginTransaction();

            var account = db.Accounts.Find(userId);
            var transaction = db.Transactions.Find(orderId);

            if (account != null && transaction == null && account.Balance >= amount)
            {
                Thread.Sleep(1000); // это чтобы наглядно показать что оплата не сразу проходит
                account.Balance -= amount;
                db.SaveChanges();

                var body = Encoding.UTF8.GetBytes(orderId + "|SUCCESS");
                var props = channel.CreateBasicProperties();
                props.Persistent = true;
                channel.BasicPublish("", "payment_results", props, body);
            } else
            {
                var body = Encoding.UTF8.GetBytes(orderId + "|FAIL");
                var props = channel.CreateBasicProperties();
                props.Persistent = true;
                channel.BasicPublish("", "payment_results", props, body);
            }
            channel.BasicAck(ea.DeliveryTag, false);
            session.Commit();
        };

        channel.BasicConsume(
            queue: "payment_requests",
            autoAck: false,
            consumer: consumer
        );

        return Task.CompletedTask;
    }
}
