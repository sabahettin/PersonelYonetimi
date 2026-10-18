using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistance.Context
{
    static class Configuration
    {
        static public string ConnectionString
        {
            get
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                return configuration.GetConnectionString("DefaultConnection")!;
            }
        }
    }
}
