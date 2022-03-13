using BEIMA.Backend.StorageService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.Test.StorageService
{
    public class StorageProviderExtensions
    {
        public static IStorageProvider CreateAzureStorageProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IStorageProvider, AzureStorageProvider>();
            var serviceProivder = services.BuildServiceProvider();
            var storageProvider = serviceProivder.GetRequiredService<IStorageProvider>();
            return storageProvider;
        }

        public static IStorageProvider CreateMinioStorageProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IStorageProvider, MinioStorageProvider>();
            var serviceProivder = services.BuildServiceProvider();
            var storageProvider = serviceProivder.GetRequiredService<IStorageProvider>();
            return storageProvider;
        }
    }
}
