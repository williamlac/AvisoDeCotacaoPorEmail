using Microsoft.Extensions.Configuration;

namespace AvisoDeCotacaoPorEmail
{
    public class Configs
    {
        public  IConfigurationRoot GetAppSettings()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json");
            var config = builder.Build();
            return config;
        }
        public  IConfigurationRoot GetSmtpSettings()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("smtpSettings.json");
            var config = builder.Build();
            return config;
        }
    }
}