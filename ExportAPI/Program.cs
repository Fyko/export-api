using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace ExportAPI
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
                    // webBuilder.ConfigureKestrel(options => {
                    //     options.ListenAnyIP(5001, listenOptions => {
                    //         listenOptions.Protocols = HttpProtocols.Http2;
                    //     });

                    //     options.ListenAnyIP(5002, listenOptions => {
                    //         listenOptions.Protocols = HttpProtocols.Http1;
                    //     });
                    // });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
