using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;

namespace JaegerDemo01
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // OpenTracing Tracer    
            if (Configuration.GetSection("OpenTracing:Agent").Exists())
            {
                string agentHost = Configuration.GetSection("OpenTracing:Agent").GetValue<string>("Host");
                int agentPort = Configuration.GetSection("OpenTracing:Agent").GetValue<int>("Port");
                int agentMaxPacketSize = Configuration.GetSection("OpenTracing:Agent").GetValue<int>("MaxPacketSize");
                var reporter = new RemoteReporter.Builder()
                    .WithSender(new UdpSender(string.IsNullOrEmpty(agentHost) ? UdpSender.DefaultAgentUdpHost : agentHost,
                                              agentPort <= 0 ? UdpSender.DefaultAgentUdpCompactPort : agentPort,
                                              agentMaxPacketSize <= 0 ? 0 : agentMaxPacketSize))
                    .Build();
                ITracer tracer = new Tracer.Builder(Assembly.GetEntryAssembly().GetName().Name)  // service name
                   .WithReporter(reporter)
                   .WithSampler(new ConstSampler(true))  // always send the span
                   .Build();
                GlobalTracer.Register(tracer);

                // ע��Tracer
                services.AddSingleton(tracer);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseJager(new JaegerOptions { QuerySpan = true, QueryValueMaxLength = 50 });
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
