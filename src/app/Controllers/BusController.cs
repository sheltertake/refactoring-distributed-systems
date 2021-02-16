using System;
using System.Linq;
using System.Threading.Tasks;
using app.Context;
using app.Entities;
using app.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BusController : ControllerBase
    {
        private readonly CartContext dbContext;
        private readonly IBusService busService;
        private readonly ILogger<BusController> _logger;

        public BusController(CartContext dbContext,
                              IBusService busService,
                              ILogger<BusController> logger)
        {
            this.dbContext = dbContext;
            this.busService = busService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            var orders = await dbContext.Orders.Where(x => !x.IsEventSent).ToListAsync();
            foreach(var order in orders)
            {
                try
                {
                    await busService.Publish(order);
                    order.IsEventSent = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
            await dbContext.SaveChangesAsync();
            return Ok();
        }

    }
}
