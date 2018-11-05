using System;
using EasyNetQ;
using LotteryFun.Web.MessageService;
using LotteryFun.Web.Models;
using LotteryFun.Web.MQ;
using LotteryFun.Web.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace LotteryFun.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            AppConfig config = Configuration.GetSection("AppConfig").Get<AppConfig>();

            services.AddHttpContextAccessor();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton(config);

            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(config.TgBotToken));
            services.AddScoped<IMessageService, TelegramBotService>();

            return services.RegistAutofacServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime lifetime,
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }


            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();


            // 开启订阅计划号码
            var consumer = serviceProvider.GetService<LotteryPlanConsumer>();
            lifetime.ApplicationStarted.Register(consumer.Subscribe);

            //            var messageService = serviceProvider.GetService<IMessageService>();
//            await messageService.Send("hi");
        }
    }
}