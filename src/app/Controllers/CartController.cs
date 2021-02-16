using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using app.Context;
using app.Entities;
using app.Models;
using app.Requests;
using app.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private static int COUNTER = 0;
        private readonly CartContext dbContext;
        private readonly IMailerService mailerService;
        private readonly IPayService payService;
        private readonly IBusService busService;
        private readonly ILogger<CartController> _logger;

        public CartController(CartContext dbContext,
                              IMailerService mailerService,
                              IPayService payService,
                              IBusService busService,
                              ILogger<CartController> logger)
        {
            this.dbContext = dbContext;
            this.mailerService = mailerService;
            this.payService = payService;
            this.busService = busService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ReportResponse>> GetAsync()
        {
            var orders = await dbContext.Orders.ToListAsync();
            var payments = await payService.GetAsync();
            var mails =  await mailerService.GetAsync();
            var events = await busService.GetAsync();
            return Ok(new ReportResponse
            {
                Counters = new Report
                {
                    Orders = orders.Count,
                    Payments = payments.Count(),
                    Mails = mails.Count(),
                    Events = events.Count(),

                },
                Duplicates = new Report
                {
                    Payments = payments.GroupBy(x => x.OrderId).Count(grp => grp.Count() > 1),
                    Mails = mails.GroupBy(x => x.OrderId).Count(grp => grp.Count() > 1),
                    Events = events.GroupBy(x => x.OrderId).Count(grp => grp.Count() > 1)
                },
                Errors = new Report
                {
                    Orders = COUNTER - orders.Count
                }
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        public async Task<ActionResult<Order>> PostAsync(Cart model)
        {
            Interlocked.Increment(ref COUNTER);
            var order = await CreateOrderAsync(model);
            await payService.PostPaymentAsync(order);
            await mailerService.SendPaymentSuccessEmailAsync(order);
            await busService.Publish(order);
            return Created(Request.Path, order);
        }

        private async Task<Order> CreateOrderAsync(Cart model)
        {
            var newOrder = new Order() { CustomerId = model.CustomerId };
            await dbContext.Orders.AddAsync(newOrder);


            await dbContext.SaveChangesAsync();
            return newOrder;
        }
    }
}
