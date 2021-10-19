using Identity.API.Data;
using Identity.API.Models;
using Identity.API.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;

namespace Identity.API
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
            services.AddControllersWithViews();

            var connectionString = Configuration["ConnectionString"];
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationAssembly);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);

                    });
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            services.Configure<AppSettings>(Configuration);

            /*
            if (Configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            {
                services.AddDataProtection(options =>
                {
                    options.ApplicationDiscriminator = "eshop.identity";
                });
            }

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddSqlServer()
            
            */

            services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
            services.AddTransient<IRedirectService, RedirectService>();

            services.AddIdentityServer(options =>
            {
                options.IssuerUri = "null";
                options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
                //options.Events.RaiseErrorEvents = true;
                //options.Events.RaiseInformationEvents = true;
                //options.Events.RaiseFailureEvents = true;
                //options.Events.RaiseSuccessEvents = true;
            })
            .AddAspNetIdentity<ApplicationUser>()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationAssembly);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,

                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationAssembly);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            })
            .AddDeveloperSigningCredential()
            .Services.AddTransient<IProfileService, ProfileService>();

            services.AddControllers();
            services.AddControllersWithViews();
            services.AddRazorPages();

            //var container = new ContainerBuilder();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app, env);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var configContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configContext.Database.Migrate();
                new ConfigurationDbContextSeed().SeedAsync(configContext, Configuration).Wait();
                var identityDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                identityDbContext.Database.Migrate();
                var settings = serviceScope.ServiceProvider.GetRequiredService<IOptions<AppSettings>>();
                new ApplicationDbContextSeed().SeedAsync(identityDbContext).Wait();
            }
        }
    }
}