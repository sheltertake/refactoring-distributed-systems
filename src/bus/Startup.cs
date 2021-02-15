using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace bus
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/", context =>
                {
                    context.Response.StatusCode = 200;
                    return Task.CompletedTask;
                });
            });
        }
    }
}
