using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Colin.Lottery.WebApp.Hubs;
using Colin.Lottery.WebApp.Services;
using Colin.Lottery.DataService;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;

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

            //services.AddSignalR(hubOptions =>
            //{
            //    hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(10);
            //})
            //.AddMessagePackProtocol();
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
                //routes.MapHub<NotifyHub>("/hubs/notify");
            });

            _provider = app.ApplicationServices;
            _pk10Context= GetService<IHubContext<PK10Hub>>();

            //启动策略
            var service = JinMaDataService.Instance;
            service.DataCollectedSuccess += Service_DataCollectedSuccess;
            service.DataCollectedError += Service_DataCollectedError;
            await service.Start();
        }

        private async void Service_DataCollectedSuccess(object sender, DataCollectedEventArgs e)
        {
            //1.推送完整15期计划
            await _pk10Context.Clients.Group(e.Rule.ToString()).SendAsync("ShowPlans",e.Plans);

            //2.推送最新期计划
            await _pk10Context.Clients.Group("AllRules").SendAsync("ShowPlans",e.LastForcastData);
        }

        private async void Service_DataCollectedError(object sender, CollectErrorEventArgs e)
        {
            await _pk10Context.Clients.Groups(new List<string> { e.Rule.ToString(), "AllRules" }).SendAsync("NoResult", e.Rule.ToStringName());
            LogUtil.Warn("目标网站扫水接口异常，请尽快检查恢复");
        }

        
        private static IServiceProvider _provider;
        public static T GetService<T>() where T : class
        {
            return _provider.GetService(typeof(T)) as T;
        }
        private static IHubContext<PK10Hub> _pk10Context;
    }
}
