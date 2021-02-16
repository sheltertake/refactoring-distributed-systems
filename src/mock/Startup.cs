using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace mock
{

    public class Startup
    {
        private static int COUNTER;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(sp => new MockContext(new DbContextOptionsBuilder<MockContext>().UseInMemoryDatabase(databaseName: "mock").Options));
        }

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

                    var body = await JsonSerializer.DeserializeAsync<OrderRequest>(context.Request.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var dbContext = context.RequestServices.GetRequiredService<MockContext>();
                    await dbContext.Orders.AddAsync(body);
                    await dbContext.SaveChangesAsync();
                    context.Response.StatusCode = 200;
                });
                endpoints.MapGet("/", async context =>
                {
                    var dbContext = context.RequestServices.GetRequiredService<MockContext>();
                    var items = await dbContext.Orders.ToListAsync();
                    await context.Response.WriteAsJsonAsync(new MockReportResponse
                    {
                        Counter = COUNTER,
                        Items = items,
                        Errors = COUNTER - items.Count
                    });
                });
            });
        }
    }

    public class MockContext : DbContext
    {
        public MockContext(DbContextOptions options) : base(options)
        {
        }
        public virtual DbSet<OrderRequest> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderRequest>(entity =>
            {
                entity.HasKey(x => x.Id);
            });
        }
    }
    public class MockReportResponse
    {
        public int Counter { get; set; }
        public int Errors { get; set; }
        public IEnumerable<OrderRequest> Items { get; set; }
    }
    public class OrderRequest : Order
    {
        public int Id { get; set; }
    }
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
    }
}
