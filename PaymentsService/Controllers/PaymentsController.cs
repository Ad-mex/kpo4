using Microsoft.AspNetCore.Mvc;
using PaymentsService.Data;
using PaymentsService.Models;
namespace PaymentsService.Controllers;

[ApiController]
public class PaymentsController(AppDbContext dbContext) : ControllerBase
{
    private readonly AppDbContext db = dbContext;

    [HttpPost("create_account")]
    public IActionResult CreateAccount([FromQuery] string user_id)
    {
        if (db.Accounts.Find(user_id) != null) return Conflict();

        db.Accounts.Add(new Account { Id = user_id, Balance = 0 });
        db.SaveChanges();
        return Created();
    }

    [HttpPost("add_money")]
    public IActionResult AddMoney([FromQuery] string user_id, [FromQuery] decimal amount)
    {
        var account = db.Accounts.Find(user_id);
        if (account == null) return NotFound();
        if (amount <= 0) return BadRequest();

        account.Balance += amount;
        db.SaveChanges();
        return Ok();
    }

    [HttpGet("balance")]
    public ActionResult<decimal> GetBalance([FromQuery] string user_id)
    {
        var account = db.Accounts.Find(user_id);
        if (account == null) return NotFound();
        return Ok(account.Balance);
    }
}
