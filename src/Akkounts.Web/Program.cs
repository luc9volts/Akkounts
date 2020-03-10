using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Akkounts.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // .ConfigureLogging(logging =>
                // {
                //     logging.ClearProviders();
                //     logging.AddConsole();
                //     logging.AddDebug();
                // })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}