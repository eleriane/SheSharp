﻿using System.Linq;
using System.Threading.Tasks;
using DotNetConf.Data;
using DotNetConf.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetConf.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]

    public class CustomersController : ControllerBase
    {
        private readonly DotNetConfContext _ctx;

        public CustomersController(DotNetConfContext ctx)
        {
            _ctx = ctx;
        }

        // GET api/values
        [HttpGet]
        public ActionResult GetAll()
        {
            return Ok(_ctx.Customers
              .Include(c => c.Address)
              //.Include(c => c.Orders).ThenInclude(o => o.Items).ThenInclude(i => i.Product)
              .OrderBy(c => c.Name)
              .ToList());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        public ActionResult Get(int id)
        {
            var customer = _ctx.Customers
              .Include(c => c.Address)
              //.Include(c => c.Orders).ThenInclude(o => o.Items).ThenInclude(i => i.Product)
              .OrderBy(c => c.Name)
              .Where(c => c.Id == id)
              .FirstOrDefault();

            if (customer == null) return NotFound();

            return Ok(customer);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Customer model)
        {
            if (ModelState.IsValid)
            {
                _ctx.Add(model);
                if (await _ctx.SaveChangesAsync() > 0)
                {
                    return Created($"/api/customers/{model.Id}", model);
                }
            }

            return BadRequest();
        }
    }
}
