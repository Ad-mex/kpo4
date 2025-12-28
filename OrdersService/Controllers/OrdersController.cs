using Microsoft.AspNetCore.Mvc;
using OrdersService.Data;
using Common;
using OrdersService.Messaging;
namespace OrdersService.Controllers;

[ApiController]
public class OrdersController(AppDbContext dbContext, OrdersPublisher publisherContext) : ControllerBase
{
    private readonly AppDbContext db = dbContext;
    private readonly OrdersPublisher publisher = publisherContext;

    [HttpGet("orders")]
    public ActionResult<List<Order>> GetOrders([FromQuery] string user_id)
    {
        var orders = db.Orders.Where(o => o.UserId == user_id).ToList();
        return Ok(orders);
    }

    [HttpGet("status")]
    public ActionResult<string> GetOrderStatus([FromQuery] string order_id)
    {
        var order = db.Orders.Find(order_id);
        return order == null ? NotFound() : Ok(order.Status.ToString());
    }

    [HttpPost("create_order")]
    public ActionResult<string> CreateOrder([FromQuery] string user_id, [FromQuery] decimal amount, [FromQuery] string? description)
    {
        var order = new Order(user_id, amount, description);
        db.Orders.Add(order);
        db.SaveChanges();

        publisher.PublishPaymentRequest(order.Id, user_id, amount);

        return Ok(order.Id);
    }
}
