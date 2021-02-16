using System;
using System.Threading;
using System.Threading.Tasks;
using app.Context;
using app.Entities;
using app.Requests;
using app.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        public static int COUNTER;
        public static int ERRORS;
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

        [HttpPost]
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        public async Task<ActionResult<Order>> PostAsync(Cart model)
        {
            Interlocked.Increment(ref COUNTER);
            try
            {
                var order = await CreateOrderAsync(model);
                await payService.PostPaymentAsync(order);
                await mailerService.SendPaymentSuccessEmailAsync(order);
                await busService.Publish(order);
                return Created(Request.Path, order);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref ERRORS);
                _logger.LogError(ex, ex.Message);
                return Problem(title: ex.Message);
            }
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
