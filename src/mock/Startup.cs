using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace mock
{

    public class Startup
    {
        private static int COUNTER;
        private static ConcurrentBag<Order> Orders = new ConcurrentBag<Order>();
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogInformation(Environment.GetEnvironmentVariable("MOCK_RANDOM"));

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/", async context =>
                {
                    Interlocked.Increment(ref COUNTER);

                    int.TryParse(Environment.GetEnvironmentVariable("MOCK_RANDOM"), out int result);
                    if (result > 0)
                    {
                        var rnd = new Random().Next(0, result);
                        if (rnd == 0)
                        {
                            throw new Exception();
                        }
                    }

                    var body = await JsonSerializer.DeserializeAsync<Order>(context.Request.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    Orders.Add(body);
                    context.Response.StatusCode = 200;
                });
                endpoints.MapGet("/", async context =>
                {                    
                    var items = Orders;
                    await context.Response.WriteAsJsonAsync(new MockReportResponse
                    {
                        Counter = COUNTER,
                        Items = items.ToArray(),
                        Errors = COUNTER - items.Count
                    });
                });
            });
        }
    }

    public class MockReportResponse
    {
        public int Counter { get; set; }
        public int Errors { get; set; }
        public IEnumerable<Order> Items { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
    }

    /*
     * 
    // WRITE
    //var dbContext = context.RequestServices.GetRequiredService<MockContext>();
    //await dbContext.Orders.AddAsync(new OrderRequest
    //{
    //    CustomerId = body.CustomerId,
    //    OrderId = body.OrderId,
    //});
    //await dbContext.SaveChangesAsync();

    //READ
    //var dbContext = context.RequestServices.GetRequiredService<MockContext>();
    //var items = await dbContext.Orders.ToListAsync();

     */

    //public void ConfigureServices(IServiceCollection services)
    //{
    //    services.AddScoped(sp => new MockContext(new DbContextOptionsBuilder<MockContext>().UseInMemoryDatabase(databaseName: "mock").Options));
    //}

    //public class OrderRequest : Order
    //{
    //    public Guid Guid { get; } = Guid.NewGuid();
    //}


    //public class MockContext : DbContext
    //{
    //    public MockContext(DbContextOptions options) : base(options)
    //    {
    //    }
    //    public virtual DbSet<OrderRequest> Orders { get; set; }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<OrderRequest>(entity =>
    //        {
    //            //entity.HasNoKey();
    //            entity.HasKey(x => x.Guid);
    //        });
    //    }
    //}
}
