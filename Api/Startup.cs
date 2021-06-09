using FridgePull.InfluxDb;
using FridgePull.InfluxDb.Options;
using InfluxDB.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FridgePull.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<InfluxDbOptions>(Configuration.GetSection(InfluxDbOptions.InfluxDb));
            services.Configure<BierCoolOptions>(Configuration.GetSection(BierCoolOptions.BierCool));
            
            services.AddTransient(_ => InfluxDBClientFactory.Create(Configuration["InfluxDb:Host"], Configuration["InfluxDb:Token"]));
            services.AddScoped<MeasurementRepository>();
            services.AddScoped<MeasurementService>();
            
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}