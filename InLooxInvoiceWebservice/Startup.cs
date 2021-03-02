using InLooxInvoiceWebservice.Logic;
using InLooxInvoiceWebservice.Models;
using InLooxInvoiceWebservice.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InLooxInvoiceWebservice
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureContainer(services);

            services.AddControllers();
        }

        private static void ConfigureContainer(IServiceCollection services)
        {
            services.AddScoped<IService, InvoiceService>();
            services.AddScoped<IWorkflow, InvoiceWorkflow>();
            services.AddTransient<IEntityModel, InvoiceModel>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
