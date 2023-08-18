using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Abaci.Subtitles.Tests
{
    public class SecretAppsettingReader
    {
        public IConfigurationSection ReadSection(string sectionName)
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<SecretAppsettingReader>();
            var configurationRoot = builder.Build();

            return configurationRoot.GetSection(sectionName);
        }
    }
}
