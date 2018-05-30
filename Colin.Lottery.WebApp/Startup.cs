using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Colin.Lottery.WebApp.Hubs;
using Colin.Lottery.WebApp.Services;

namespace Colin.Lottery.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSignalR();
            //services.AddScoped<PK10Hub>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //TODO: 暂不使用Https
            //app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            //app.UseAuthentication();UseAuthentication

            app.UseMvc();

            app.UseSignalR(routes =>
            {
                routes.MapHub<PK10Hub>("/hubs/pk10");
                routes.MapHub<NotifyHub>("/hubs/notify");
            });

            _provider = app.ApplicationServices;

            //启动策略
            await JinMaStrategyService.Instance.Start();
        }


        private static IServiceProvider _provider;
        public static T GetService<T>() where T : class
        {
            return _provider.GetService(typeof(T)) as T;
        }


    }
}
