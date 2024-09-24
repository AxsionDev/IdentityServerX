using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using IdentityServerX.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore;
using IdentityServerX.EntityFramework;
using IdentityServerX.EntityFramework.DbContexts;
using System.Linq;

namespace ConsoleHost
{
    class Program
    {
        static  void Main(string[] args)
        {
            var connectionString = "Data Source=.;Initial Catalog=isxTest1;User ID=sa;Password=Peter01!;MultipleActiveResultSets=true";

            //simpleTest(connectionString);


            var services = new ServiceCollection();

            services.AddOperationalDbContext(options => {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString, dbOpts => dbOpts.MigrationsAssembly("ConsoleHost")).EnableSensitiveDataLogging();
            });

            services.AddConfigurationDbContext(options => {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString, dbOpts => dbOpts.MigrationsAssembly("ConsoleHost")).EnableSensitiveDataLogging();
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                    var ctx = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                    ctx.Database.Migrate();
                    ctx.SaveChanges();

                    var t =  ctx.Clients.ToList();

            }
        }

        private static void simpleTest(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));
            services.AddOperationalDbContext(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);

                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = false;
                options.TokenCleanupInterval = 5; // interval in seconds, short for testing
            });

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var svc = scope.ServiceProvider.GetRequiredService<TokenCleanupService>();
                svc.RemoveExpiredGrantsAsync().GetAwaiter().GetResult();
            }
        }
    }
}
