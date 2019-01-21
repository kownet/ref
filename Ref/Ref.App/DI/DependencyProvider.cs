using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ref.App.Core;
using Ref.App.Options;
using Ref.Data.Repositories;
using Ref.Shared.Common;
using Ref.Shared.Notifications;
using Ref.Sites;
using System;

namespace Ref.App.DI
{
    public static class DependencyProvider
    {
        public static IServiceProvider Get(IConfigurationRoot configurationRoot, string clientId)
        {
            var services = new ServiceCollection();

            #region Client
            services.AddTransient<IClientProvider>(s => new ClientProvider(clientId));
            #endregion

            #region Storages
            services.AddTransient<IStorageProvider>(
                s => new StorageProvider(
                    configurationRoot["storages:file_json:dir"],
                    clientId)
                );
            #endregion

            #region Notifications
            services.AddTransient<IPushOverProvider>(
                s => new PushOverProvider(
                    configurationRoot["notifications:pushover:token"],
                    configurationRoot["notifications:pushover:recipients"],
                    configurationRoot["notifications:pushover:endpoint"])
                );

            services.AddTransient<IPushOverNotification, PushOverNotification>();
            #endregion

            #region Repositories
            services.AddTransient<IAdRepository, JsonRepository>();
            #endregion

            #region Filters
            services.Configure<FilterConfiguration>(
                opt => configurationRoot
                .GetSection("filter")
                .Bind(opt));
            #endregion

            #region Sites
            services.AddTransient<ISite, OtoDomSite>();
            #endregion

            services.AddTransient<RefService>();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}