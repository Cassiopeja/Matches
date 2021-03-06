using Matches.Hubs;
using Matches.Infrastructure;
using Matches.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VueCliMiddleware;

namespace Matches
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDateTimeService, DateTimeService>();
            services.AddSingleton<IGameManager, GameManager>();
            services.AddControllers();
            services.AddSignalR(opt =>
                opt.EnableDetailedErrors = true);
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAny", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddAutoMapper(typeof(Startup));

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist";});
            services.AddHostedService<TemplateLoadService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (!env.IsDevelopment()) app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/gameHub");
                // NOTE: VueCliProxy is meant for development and hot module reload
                // NOTE: SSR has not been tested
                // Production systems should only need the UseSpaStaticFiles() (above)
                // You could wrap this proxy in either
                // if (System.Diagnostics.Debugger.IsAttached)
                // or a preprocessor such as #if DEBUG
                // if (!Debugger.IsAttached)
                //     endpoints.MapToVueCliProxy(
                //         "{*path}",
                //         new SpaOptions {SourcePath = "ClientApp"},
                //         Debugger.IsAttached ? "serve" : null,
                //         regex: "Compiled successfully",
                //         forceKill: true
                //     );
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
            
            if (env.IsDevelopment())
                // run npm process with client app
                spa.UseVueCli("serve", 8080);
            // if you just prefer to proxy requests from client app, use proxy to SPA dev server instead,
            // app should be already running before starting a .NET client:
            // spa.UseProxyToSpaDevelopmentServer("http://localhost:8080"); // your Vue app port
            });
        }
    }
}