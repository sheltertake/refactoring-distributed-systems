using System;
using System.Threading.Tasks;
using app.Context;
using app.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "app", Version = "v1" });
            });

            services.AddSingleton(sp => new CartContext(new DbContextOptionsBuilder<CartContext>().UseInMemoryDatabase(databaseName: "test").Options));
            services.AddSingleton<IMailerService, MailerService>();
            services.AddSingleton<IBusService, BusService>();
            services.AddSingleton<IPayService, PayService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "app v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapPost("/", context =>
                {
                    context.Response.StatusCode = 200;
                    return Task.CompletedTask;
                    //var name = context.Request.RouteValues["name"];
                    //await context.Response.WriteAsync($"Hello {name}!");
                });
            });
        }
    }
}
