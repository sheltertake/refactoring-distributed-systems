using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace mock
{

    public class Startup
    {
        //public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var options = Configuration.GetSection("AppSettings");

            //services.AddControllers();
            //services.AddHttpClient(nameof(MailerService), c => c.BaseAddress = options.GetValue<Uri>("MailerUrl"));
            //services.AddHttpClient(nameof(BusService), c => c.BaseAddress = options.GetValue<Uri>("BusUrl"));
            //services.AddHttpClient(nameof(PayService), c => c.BaseAddress = options.GetValue<Uri>("PayUrl"));

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "app", Version = "v1" });
            //});

            services.AddSingleton(sp => new MockContext(new DbContextOptionsBuilder<MockContext>().UseInMemoryDatabase(databaseName: "mock").Options));
            //services.AddSingleton<IMailerService, MailerService>();
            //services.AddSingleton<IBusService, BusService>();
            //services.AddSingleton<IPayService, PayService>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/", async context =>
                {
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
                    await context.Response.WriteAsJsonAsync(items);
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
