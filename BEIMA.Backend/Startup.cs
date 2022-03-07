using BEIMA.Backend.StorageService;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]

namespace MyNamespace
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var environment = Environment.GetEnvironmentVariable("CurrentEnv");

            
            if (environment == "dev-local")
            {
                builder.Services.AddSingleton<IStorageProvider, MinioStorageProvider>();
            } else
            {
                builder.Services.AddSingleton<IStorageProvider, AzureStorageProvider>();
            }
            
        }
    }
}