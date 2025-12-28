using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using OrdersService.Data;
using Common;

namespace OrdersService.Messaging;

public class PaymentsConsumer : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IModel channel;

    public PaymentsConsumer(IServiceScopeFactory scopeFactory, IModel channel)
    {
        this.scopeFactory = scopeFactory;
        this.channel = channel;

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
            var status = parts[1];

            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var order = db.Orders.Find(orderId);
            if (order != null) 
            {
                if(status == "SUCCESS")
                {
                    order.Status = OrderStatus.FINISHED;
                } else
                {
                    order.Status = OrderStatus.CANCELLED;
                }
                db.SaveChanges();
            }

            channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(
            queue: "payment_results",
            autoAck: false,
            consumer: consumer
        );

        return Task.CompletedTask;
    }
}
