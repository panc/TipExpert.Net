using System.IO;
using AutoMapper;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using TipExpert.Net.Authentication;
using TipExpert.Net.Middleware;
using TipExpert.Core;
using TipExpert.Net.Models;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace TipExpert.Net
{
    public class Startup
    {
        private string _appDataPath;

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
            //if (env.IsDevelopment())
            //    builder.AddUserSecrets();

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Identity services to the services container.
            services.AddIdentity<ApplicationUser, ApplicationIdentityRole>()
                .AddUserStore<ApplicationUserStore>()
                .AddRoleStore<ApplicationRoleStore>();

            services.AddSingleton<IUserStore, UserStore>(s => new UserStore(_appDataPath));
            services.AddSingleton<ILeagueStore, LeagueStore>(s => new LeagueStore(_appDataPath));
            services.AddSingleton<IMatchStore, MatchStore>(s => new MatchStore(_appDataPath));

            // Add MVC services to the services container.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var appenvironment = (IApplicationEnvironment)app.ApplicationServices.GetService(typeof(IApplicationEnvironment));
            _appDataPath = Path.Combine(appenvironment.ApplicationBasePath, "App_Data");

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // sends the request to the following path or controller action.
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline.
            app.UseIdentity();

            app.UseRedirectOfInvalidRequests("/api/", "/");

            // Add authentication middleware to the request pipeline. You can configure options such as Id and Secret in the ConfigureServices method.
            // For more information see http://go.microsoft.com/fwlink/?LinkID=532715
            //app.UseFacebookAuthentication(options =>
            //{
            //    options.AppId = Configuration["Authentication:Facebook:AppId"];
            //    options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            //});

            // Add MVC to the request pipeline.
            app.UseMvc(
                routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });

            _SetupMapper();
        }

        private void _SetupMapper()
        {
            Mapper.Initialize(c =>
            {
                c.CreateMap<User, UserDto>();
                c.CreateMap<League, LeagueDto>();
                c.CreateMap<LeagueDto, League>();
                c.CreateMap<Match, MatchDto>();
                c.CreateMap<MatchDto, Match>();
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
