using Microsoft.AspNetCore.Mvc;
using Common;
using Microsoft.AspNetCore.WebUtilities;

namespace ApiGateway.Controllers
{
    [ApiController]
    public class UserController(IHttpClientFactory factory) : ControllerBase
    {
        private readonly HttpClient paymentsClient = factory.CreateClient("Payments");
        private readonly HttpClient ordersClient = factory.CreateClient("Orders");

        [HttpGet("user_id")]
        public ActionResult<string> GetId()
        {
            return Ok(IdGenerator.Get());
        }

        private IActionResult Proxy(HttpClient client, string method, string path, Dictionary<string, string?> queryParams)
        {
            var uri = QueryHelpers.AddQueryString(path, queryParams);
            HttpResponseMessage? response = null;

            if (method == "GET") {
                response = client.GetAsync(uri).GetAwaiter().GetResult();
            } else if(method == "POST") {
                response = client.PostAsync(uri, null).GetAwaiter().GetResult();
            } else {
                return BadRequest();
            }

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return StatusCode((int)response.StatusCode, content);
        }

        [HttpPost("create_account")]
        public IActionResult CreateAccount([FromQuery(Name = "user_id")] string userId)
        {
            return Proxy(paymentsClient, "POST", "create_account", new Dictionary<string, string?>
            {
                ["user_id"] = userId
            });
        }

        [HttpPost("add_money")]
        public IActionResult AddMoney([FromQuery(Name = "user_id")] string userId, [FromQuery] decimal amount)
        {
            return Proxy(paymentsClient, "POST", "add_money", new Dictionary<string, string?>
            {
                ["user_id"] = userId,
                ["amount"] = amount.ToString()
            });
        }

        [HttpGet("balance")]
        public IActionResult GetBalance([FromQuery(Name = "user_id")] string userId)
        {
            return Proxy(paymentsClient, "GET", "balance", new Dictionary<string, string?>
            {
                ["user_id"] = userId
            });
        }

        [HttpGet("orders")]
        public IActionResult GetOrders([FromQuery(Name = "user_id")] string userId)
        {
            return Proxy(ordersClient, "GET", "orders", new Dictionary<string, string?>
            {
                ["user_id"] = userId
            });
        }

        [HttpGet("status")]
        public IActionResult GetStatus([FromQuery(Name = "order_id")] string orderId)
        {
            return Proxy(ordersClient, "GET", "status", new Dictionary<string, string?>
            {
                ["order_id"] = orderId
            });
        }

        [HttpPost("create_order")]
        public IActionResult CreateOrder([FromQuery(Name = "user_id")] string userId, [FromQuery(Name = "amount")] decimal amount, [FromQuery(Name = "description")] string? description)
        {
            return Proxy(ordersClient, "POST", "create_order", new Dictionary<string, string?>
            {
                ["user_id"] = userId,
                ["amount"] = amount.ToString(),
                ["description"] = description
            });
        }
    }
}
