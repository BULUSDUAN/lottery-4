using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Colin.Lottery.Utils
{
    public class ConfigUtil
    {
        public static IConfiguration Configuration { get; private set; }
        static ConfigUtil()
        {
            Configuration = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
                .Build();            
        }
    }
}