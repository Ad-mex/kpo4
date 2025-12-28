using RabbitMQ.Client;
using System.Text;

namespace OrdersService.Messaging;

public class OrdersPublisher
{
    private readonly IModel channel;

    public OrdersPublisher(IModel channel)
    {
        this.channel = channel;

        this.channel.QueueDeclare(
            queue: "payment_requests",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    public void PublishPaymentRequest(string orderId, string userId, decimal amount)
    {
        var message = $"{orderId}|{userId}|{amount}";
        var body = Encoding.UTF8.GetBytes(message);

        var props = channel.CreateBasicProperties();
        props.Persistent = true;

        channel.BasicPublish(
            exchange: "",
            routingKey: "payment_requests",
            basicProperties: props,
            body: body
        );
    }
}
