using System.Threading.Tasks;
using app.Context;
using app.Entities;
using app.Events;
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
        public async Task<ActionResult<Order>> GetAsync(Cart model)
        {
            var order = await CreateOrderAsync(model);
            await payService.PostPaymentAsync(order);
            await mailerService.SendPaymentSuccessEmailAsync(order);
            await busService.Publish(new OrderCreatedEvent { Id = order.OrderId });
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
