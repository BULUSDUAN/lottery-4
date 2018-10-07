using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Colin.Lottery.WebApp.Hubs;
using Colin.Lottery.DataService;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;
using Microsoft.AspNetCore.HttpOverrides;
using System.Linq;

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
        public async void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDistributedMemoryCache();

            // AddSession 必须在 AddMvc() 之前执行
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
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

            //启动策略
            var service = JinMaDataService.Instance;
            service.DataCollectedSuccess += Service_DataCollectedSuccess;
            service.DataCollectedError += Service_DataCollectedError;
            await service.Start();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();

            //app.UseAuthentication();UseAuthentication

            app.UseMvc();

            app.UseSignalR(routes =>
            {
                routes.MapHub<PK10Hub>("/hubs/pk10");
                //routes.MapHub<NotifyHub>("/hubs/notify");
            });

            _provider = app.ApplicationServices;
            _pk10Context = GetService<IHubContext<PK10Hub>>();
        }

        private async void Service_DataCollectedSuccess(object sender, DataCollectedEventArgs e)
        {
            //1.推送完整15期计划
            await _pk10Context.Clients.Group(e.Rule.ToString()).SendAsync("ShowPlans", e.Plans);

            //2.推送最新期计划
            await _pk10Context.Clients.Group("AllRules").SendAsync("ShowPlans", e.LastForcastData);

            //3.推送App消息
            if ((int)e.Rule <= 4)
            {
                var plan = e.LastForcastData;
                if (plan.Any() && plan.LastOrDefault().KeepGuaCnt >= Convert.ToInt32(ConfigUtil.Configuration["AppNotify:Min"]))
                    await _pk10Context.Clients.Group("App").SendAsync("ShowPlans", plan);
            }
        }

        private static async void Service_DataCollectedError(object sender, CollectErrorEventArgs e)
        {
            await _pk10Context.Clients.Groups(new List<string> { e.Rule.ToString(), "AllRules" })
                .SendAsync("NoResult", e.Rule.ToStringName());
            LogUtil.Warn("目标网站扫水接口异常，请尽快检查恢复");
        }


        private static IServiceProvider _provider;

        private static T GetService<T>() where T : class
        {
            return _provider.GetService(typeof(T)) as T;
        }

        private static IHubContext<PK10Hub> _pk10Context;
    }
}