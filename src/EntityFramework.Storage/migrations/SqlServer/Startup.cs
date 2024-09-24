using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using IdentityServerX.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore;
using IdentityServerX.EntityFramework.DbContexts;
using System.Linq;

namespace SqlServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var cn = Configuration.GetConnectionString("db");

            services.AddOperationalDbContext(options => {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(cn, dbOpts => dbOpts.MigrationsAssembly(typeof(Startup).Assembly.FullName));
            });

            services.AddConfigurationDbContext(options => {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(cn, dbOpts => dbOpts.MigrationsAssembly(typeof(Startup).Assembly.FullName));
            });
        }

        public void Configure(IApplicationBuilder app, ConfigurationDbContext ctx)
        {
            var t = ctx.Clients.Count();
        }
    }
}
