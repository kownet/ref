using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Ref.App.Core;
using Ref.Data.Repositories;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Sites;
using System;
using System.Linq;
using System.Reflection;

namespace Ref.App.DI
{
    public static class DependencyProvider
    {
        public static IServiceProvider Get(IConfigurationRoot configurationRoot, string clientId)
        {
            var services = new ServiceCollection();

            #region Logging
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
            });
            #endregion

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

            services.AddTransient<IEmailProvider>(
                s => new EmailProvider(
                    configurationRoot["notifications:email:host"],
                    configurationRoot["notifications:email:apikey"],
                    configurationRoot["notifications:email:recipients"])
                );

            services.AddTransient<IEmailNotification, EmailNotification>();
            #endregion

            #region Repositories
            services.AddTransient<IAdRepository, JsonRepository>();
            #endregion

            #region Filters
            services.AddTransient<IFilterProvider>(
                s => new FilterProvider(
                        configurationRoot["filter:type"],
                        configurationRoot["filter:deal"],
                        configurationRoot["filter:location"],
                        configurationRoot["filter:flatareafrom"],
                        configurationRoot["filter:flatareato"],
                        configurationRoot["filter:pricefrom"],
                        configurationRoot["filter:priceto"],
                        configurationRoot["filter:market"],
                        configurationRoot["filter:newest"]
                    )
                );
            #endregion

            #region Sites
            //var siteAssembly = AppDomain.CurrentDomain.GetAssemblies()
            //    .FirstOrDefault(a => a.ManifestModule.Name == "Ref.Sites.dll");

            //siteAssembly.GetTypesAssignableFrom<ISite>().ForEach((t) =>
            //{
            //    services.AddTransient(typeof(ISite), t);
            //});

            services.AddTransient<ISite, GratkaSite>();
            #endregion

            #region App
            services.AddTransient<IAppProvider>(
                s => new AppProvider(
                    configurationRoot["app:version"],
                    configurationRoot["app:sender"],
                    configurationRoot["app:replyto"],
                    configurationRoot["app:binpath"])
                );
            #endregion

            services.AddTransient<RefService>();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}