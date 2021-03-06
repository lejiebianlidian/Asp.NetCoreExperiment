using JaegerSharp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI02
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPI02", Version = "v1" });
            });
            //����OpenTracing
            services.AddOpenTracing();
            //ע��Jaeger
            if (Convert.ToBoolean(Configuration.GetSection("OpenTracing:Enable")?.Value))
            {
                var agentHost = Configuration.GetSection("OpenTracing:Agent").GetValue<string>("Host");
                var agentPort = Configuration.GetSection("OpenTracing:Agent").GetValue<int>("Port");
                var agentMaxPacketSize = Configuration.GetSection("OpenTracing:Agent").GetValue<int>("MaxPacketSize");
                services.AddJaegerSharp(agentHost, agentPort, agentMaxPacketSize);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI02 v1"));
            }
            if (Convert.ToBoolean(Configuration.GetSection("OpenTracing:Enable")?.Value))
            {
                app.UseJaegerSharp();
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
