using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Prometheus;
using PrometheusSample.Middlewares;
using PrometheusSample.Services;

using System.Collections.Generic;

namespace PrometheusSample
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
            MetricsHandle(services);
            services.AddScoped<IOrderService, OrderService>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PrometheusSample", Version = "v1" });
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PrometheusSample v1"));
            }

            app.UseRouting();
            //http������м��
            app.UseHttpMetrics();
            app.UseAuthorization();

            //�Զ���ҵ�����
            app.UseBusinessMetrics();

            app.UseEndpoints(endpoints =>
            {
                //ӳ���ص�ַΪ  /metrics
                endpoints.MapMetrics();
                endpoints.MapControllers();
            });
        }
        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="services"></param>
        void MetricsHandle(IServiceCollection services)
        {
            var metricsHub = new MetricsHub();
            //counter
            metricsHub.AddCounter("/register", Metrics.CreateCounter("business_register_user", "ע���û�����"));
            metricsHub.AddCounter("/order", Metrics.CreateCounter("business_order_total", "�µ�������"));
            metricsHub.AddCounter("/pay", Metrics.CreateCounter("business_pay_total", "֧��������"));
            metricsHub.AddCounter("/ship", Metrics.CreateCounter("business_ship_total", "����������"));

            //gauge
            var orderGauge = Metrics.CreateGauge("business_order_count", "��ǰ�µ�������");
            var payGauge = Metrics.CreateGauge("business_pay_count", "��ǰ֧��������");
            var shipGauge = Metrics.CreateGauge("business_ship_count", "��ǰ�������ݡ�");

            metricsHub.AddGauge("/order", new Dictionary<string, Gauge> {
                { "+", orderGauge}
            });
            metricsHub.AddGauge("/pay", new Dictionary<string, Gauge> {
                {"-",orderGauge},
                {"+",payGauge}
            });
            metricsHub.AddGauge("/ship", new Dictionary<string, Gauge> {
                {"+",shipGauge},
                {"-",payGauge}
            });
            //summary �ٷ�λ��[��һ����С����������У�ĳ�����ִ���80%�����֣�������־͵�80���İٷ�λ��]
            /*0.5-quantile������0.05��0.9-quantile������0.01����0.95������0.005����0.99������0.001����Щ���������õ������̵���0.5-quantile: 0.05��˼����������������0.05������ĳ��0.5-quantile��ֵΪ120���������õ����Ϊ0.05������120�������ʵquantile��(0.45, 0.55)��Χ�ڵ�ĳ��ֵ��
             */
            var orderSummary = Metrics
     .CreateSummary("business_order_summary", "10�����ڵĶ�������",
         new SummaryConfiguration
         {
             Objectives = new[]
             {
                new QuantileEpsilonPair(0.1, 0.05),   
                new QuantileEpsilonPair(0.3, 0.05),      
                new QuantileEpsilonPair(0.5, 0.05),
                new QuantileEpsilonPair(0.7, 0.05),           
                new QuantileEpsilonPair(0.9, 0.05),
             }
         });
            metricsHub.AddSummary("/order", orderSummary);

            //histogram
            /*
             grafana��  histogram_quantile(0.95, rate(business_order_histogram_seconds_bucket[5h]))
             95%�Ķ������С�ڵ������ֵ
             */

            var orderHistogram = Metrics.CreateHistogram("business_order_histogram", "����ֱ��ͼ��",
        new HistogramConfiguration
        {
             //Buckets = Histogram.ExponentialBuckets(start: 1000, factor: 2, count: 5)
           Buckets = Histogram.LinearBuckets(start: 1000, width: 1000, count: 6)
        }) ;
         
            metricsHub.AddHistogram("/order", orderHistogram);



            services.AddSingleton(metricsHub);
        }
    }
}
