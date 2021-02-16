using System;
using System.Linq;
using System.Threading.Tasks;
using app.Context;
using app.Controllers;
using app.Models;
using app.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace app
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var options = Configuration.GetSection("AppSettings");

            services.AddControllers();
            services.AddHttpClient(nameof(MailerService), c => c.BaseAddress = options.GetValue<Uri>("MailerUrl"));
            services.AddHttpClient(nameof(BusService), c => c.BaseAddress = options.GetValue<Uri>("BusUrl"));
            services.AddHttpClient(nameof(PayService), c => c.BaseAddress = options.GetValue<Uri>("PayUrl"));
            
            services.AddScoped(sp => new CartContext(new DbContextOptionsBuilder<CartContext>()
                                                         .UseInMemoryDatabase(databaseName: "test")
                                                         .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                                                         .Options));
            services.AddSingleton<IMailerService, MailerService>();
            services.AddSingleton<IBusService, BusService>();
            services.AddSingleton<IPayService, PayService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    var dbContext = context.RequestServices.GetRequiredService<CartContext>();
                    var payService = context.RequestServices.GetRequiredService<IPayService>();
                    var mailerService = context.RequestServices.GetRequiredService<IMailerService>();
                    var busService = context.RequestServices.GetRequiredService<IBusService>();
                    var orders = await dbContext.Orders.ToListAsync();
                    var payments = await payService.GetAsync();
                    var mails = await mailerService.GetAsync();
                    var events = await busService.GetAsync();
                    await context.Response.WriteAsJsonAsync(new ReportResponse
                    {
                        CounterErrors = CartController.ERRORS,
                        Counter = CartController.COUNTER,
                        Counters = new Report
                        {
                            Orders = orders.Count,
                            Payments = payments.Items.Count(),
                            Mails = mails.Items.Count(),
                            Events = events.Items.Count(),

                        },
                        Duplicates = new Report
                        {
                            Payments = payments.Items.GroupBy(x => x.OrderId).Count(grp => grp.Count() > 1),
                            Mails = mails.Items.GroupBy(x => x.OrderId).Count(grp => grp.Count() > 1),
                            Events = events.Items.GroupBy(x => x.OrderId).Count(grp => grp.Count() > 1)
                        },
                        Errors = new Report
                        {
                            Orders = CartController.COUNTER - orders.Count,
                            Events = events.Errors,
                            Mails = mails.Errors,
                            Payments = payments.Errors,
                        },
                        Requests = new Report
                        {
                            Orders = CartController.COUNTER,
                            Events = events.Counter,
                            Mails = mails.Counter,
                            Payments = payments.Counter,
                        }
                    });
                });
            });
        }
    }
}
