using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;



namespace Colin.Lottery.Utils
{
    public static class ConfigUtil
    {
        /// <summary>
        /// 配置文件对象，读取一级节点：ConfigUtil.Configuration["nodeName"],读取多级节点：ConfigUtil.Configuration["node1:node2"]
        /// </summary>
        public static IConfiguration Configuration { get; }

        static ConfigUtil()
        {
            Configuration = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
                .Build();
        }

        /// <summary>
        /// 读取配置（强类型）
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetAppSettings<T>(string key) where T : class, new()
        { 
            return new ServiceCollection()
                .AddOptions()
                .Configure<T>(Configuration.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
        }
    }
}