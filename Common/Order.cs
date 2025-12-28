namespace Common;

public class Order
{
    public Order() { }

    public Order(string userId, decimal amount, string? description)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be positive");

        UserId = userId;
        Amount = amount;
        Description = description;
        Id = IdGenerator.Get();
    }

    public string UserId { get; set; } = null!;
    public string Id { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.NEW;
}
