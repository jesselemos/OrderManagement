using Microsoft.AspNetCore.Mvc;

namespace Order.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {

        [HttpGet(Name = "Order")]
        public string Get()
        {
            return "Hello world";
        }
    }
}