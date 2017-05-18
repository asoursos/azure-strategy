using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasyTrade.App
{
    public class Startup
    {
        // Deploy error: http://stackoverflow.com/questions/37918650/azure-web-app-deploy-web-deploy-cannot-modify-the-file-on-the-destination-becau/37938954
        internal static ADSettings AdSettings;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("config.json")
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc()
                .AddApplicationPart(typeof(Bootstrap).GetTypeInfo().Assembly);

            services.AddPcBlockServices();

            // Add Authentication services.
            services.AddAuthentication(sharedOptions => sharedOptions.SignInScheme =
                CookieAuthenticationDefaults.AuthenticationScheme);

            services.AddCors(options =>
            {
                options.AddPolicy(ADSettings.AllowSpecificOrigin, builder => builder.WithOrigins("https://login.microsoftonline.com"));
            });
            
            services.Configure<MvcOptions>(options => { options.Filters.Add(new RequireHttpsAttribute()); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Configure the OWIN pipeline to use cookie auth.
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            // App config settings
            var settings = AdSettings ?? (AdSettings = new ADSettings(Configuration));
            app.UseOpenIdConnectAuthentication(settings.CreateOptionsFromPolicy(settings.UserProfilePolicyId));
            app.UseOpenIdConnectAuthentication(settings.CreateOptionsFromPolicy(settings.SignUpOrSignInPolicyId));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}