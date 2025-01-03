using Campus_Events.Misc;
using Campus_Events.Persistence;
using Campus_Events.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Connections;
using System.Reflection;

namespace Campus_Events
{
    public class StartUp
    {
        private IConfiguration config;
        public StartUp(IConfiguration configuration) { this.config = configuration; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MailSettings>(options => config.GetSection("MailSettings").Bind(options));
            services.Configure<DatabaseSettings>(options => config.GetSection("DatabaseSettings").Bind(options));
            services.Configure<GeneralSettings>(options => config.GetSection("GeneralSettings").Bind(options));
            // services.AddSingleton<IUserRepository, UserMemoryRepository>();
            //  services.AddSingleton<IEventRepository, EventMemoryRepository>();
            // services.AddSingleton<IUserRegistration, UserRegistration>();

            // Add FluentValidation validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); 

            services.AddSingleton<DbConnectionFactory>();
            services.AddSingleton<PasswordHelper>();

            // Using EF-Core:
            services.AddScoped<ApplicationDbContext>();
            services.AddScoped<IUserRepository, UserEfCoreRepository>();
            services.AddScoped<IEventRepository, EventEfCoreRepository>();
            services.AddScoped < IUserRegistration, UserEventsEfCoreRepository>();
            // end using EF-Core /

            services.AddSingleton<EmailQueue>();
            services.AddSingleton<EmailService>();
            services.AddHostedService<BackgroundEmailSender>();

            services.AddMvc();
            services.AddProblemDetails();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Authentication/Login";
                options.AccessDeniedPath = "/Authentication/Login";
            });
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
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


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Redirection de la racine vers la page de connexion
                endpoints.MapGet("/", async context =>
                {
                    context.Response.Redirect("/Authentication/Login");
                });

                // Mappez les contrôleurs
                // Route par défaut pour contrôler l'accès aux contrôleurs et aux actions
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Authentication}/{action=Login}/{id?}");
            });
        }
    }
}

