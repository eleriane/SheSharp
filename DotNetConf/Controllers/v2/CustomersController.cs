using System.Linq;
using DotNetConf.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetConf.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]

    public class CustomersController : ControllerBase
    {
        private readonly DotNetConfContext _ctx;

        public CustomersController(DotNetConfContext ctx)
        {
            _ctx = ctx;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [MapToApiVersion("2.0")]
        public ActionResult Get(int id)
        {
            var customer = _ctx.Customers
              .Include(c => c.Address)
              .Include(c => c.Orders).ThenInclude(o => o.Items).ThenInclude(i => i.Product)
              .OrderBy(c => c.Name)
              .Where(c => c.Id == id)
              .FirstOrDefault();

            if (customer == null) return NotFound();

            return Ok(customer);
        }

    }
}
