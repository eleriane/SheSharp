using DotNetConf.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetConf
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //  restcookbook.com 

            //  martinfowler.com/articles/richardsonMaturityModel.html


            var host = BuildWebHost(args);
            SeedDb(host);
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        }

        private static void SeedDb(IWebHost host)
        {
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<DotNetConfSeeder>();
                seeder.SeedDatabaseAsync().Wait();
            }
        }
    }
}
