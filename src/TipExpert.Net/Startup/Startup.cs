using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TipExpert.Net.Authentication;
using TipExpert.Net.Middleware;
using TipExpert.Core;
using TipExpert.Net.Implementation;
using TipExpert.Core.Calculation;
using TipExpert.Core.PlayerInvitation;
using TipExpert.Core.MatchSelection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace TipExpert.Net
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", optional: true)
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
            services.AddIdentity<ApplicationUser, ApplicationIdentityRole>(
                c =>
                {
                    c.Password.RequireDigit = false;
                    c.Password.RequireUppercase = false;
                    c.Password.RequireNonLetterOrDigit = false;
                    c.Password.RequiredLength = 1;
                })
                .AddUserStore<ApplicationUserStore>()
                .AddRoleStore<ApplicationRoleStore>();

            services.AddTransient<IGameTipsUpdateManager, GameTipsUpdateManager>();
            services.AddTransient<ICalculationFactory, CalculationFactory>();
            services.AddTransient<FiveThreeOneZeroPointsCalculationStrategy>();
            services.AddTransient<TheWinneTakesItAllCalculationStrategy>();

            services.AddTransient<IMatchSelectorFactory, MatchSelectorFactory>();
            services.AddTransient<LeagueMatchSelector, LeagueMatchSelector>();
            services.AddTransient<Em2016MatchSelector, Em2016MatchSelector>();

            services.AddTransient<IPlayerInvitationService, PlayerInvitationService>(x => 
                new PlayerInvitationService(
                        x.GetService<IGameStore>(),
                        x.GetService<IInvitationStore>(),
                        Configuration["MailSettings:UserName"],
                        Configuration["MailSettings:Passwort"],
                        Configuration["MailSettings:Host"],
                        Configuration.Get<int>("MailSettings:Port")
                    ));

            services.AddSingleton(s => 
                new MongoClient(Configuration["Data:ConnectionString"])
                    .GetDatabase(Configuration["Data:DataBaseName"]));

            services.AddTransient<IUserStore, UserStore>();
            services.AddTransient<ILeagueStore, LeagueStore>();
            services.AddTransient<IMatchStore, MatchStore>();
            services.AddTransient<IGameStore, GameStore>();
            services.AddTransient<IInvitationStore, InvitationStore>();

            // Add MVC services to the services container.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
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

            MappingHelper.InitializeMappings();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
