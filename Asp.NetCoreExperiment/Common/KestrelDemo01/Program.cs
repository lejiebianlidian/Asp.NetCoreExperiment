using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KestrelDemo01
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        //�ͻ������������
                        options.Limits.MaxConcurrentConnections = 100;
                        //�����Ѵ� HTTP �� HTTPS ��������һ��Э�飨���磬Websocket ���󣩵����ӣ���һ�����������ơ�
                        options.Limits.MaxConcurrentUpgradedConnections = 100;
                        //����������������� (NULL)��
                        options.Limits.MaxRequestBodySize = 10 * 1024;
                        //����������С��������
                        options.Limits.MinRequestBodyDataRate =
                            new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                        //Ӧ������������С��������
                        options.Limits.MinResponseDataRate =
                            new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));

                        options.Listen(IPAddress.Loopback, 5000);
                        options.Listen(IPAddress.Loopback, 5001, listenOptions =>
                        {
                            listenOptions.UseHttps("testCert.pfx", "testPassword");
                        });
                        options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
                        //�����ͷ��ʱ
                        options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
                        //ͬ�� IO
                        options.AllowSynchronousIO = true;
                        //ÿ�����ӵ������
                        options.Limits.Http2.MaxStreamsPerConnection = 100;
                        //������С
                        options.Limits.Http2.HeaderTableSize = 4096;
                        //���֡��С
                        options.Limits.Http2.MaxFrameSize = 16384;
                        //��������ͷ��С
                        options.Limits.Http2.MaxRequestHeaderFieldSize = 8192;
                        //��ʼ���Ӵ��ڴ�С
                        options.Limits.Http2.InitialConnectionWindowSize = 131072;
                        //��ʼ�����ڴ�С
                        options.Limits.Http2.InitialStreamWindowSize = 98304;

                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
