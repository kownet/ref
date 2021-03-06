﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Ref.Cleaner.Core;
using Ref.Data.Db;
using Ref.Data.Repositories;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using System;

namespace Ref.Cleaner.DI
{
    public static class DependencyProvider
    {
        public static IServiceProvider Get(IConfigurationRoot configurationRoot, string appId)
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

            #region Storages
            services.AddTransient<IDbAccess>(
                s => new DbAccess(configurationRoot["storages:mssql:cs"])
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
            services.AddTransient<IOfferRepository, OfferRepository>();
            #endregion

            #region App
            services.AddTransient<IAppCleanerProvider>(
            s => new AppCleanerProvider(
                configurationRoot["app:address"],
                configurationRoot["app:sender"],
                configurationRoot["app:replyto"],
                configurationRoot["app:pausetime"],
                configurationRoot["app:adminnotification"],
                configurationRoot["app:successtries"],
                appId,
                configurationRoot["app:daystolive"])
            );
            #endregion

            services.AddTransient<CleanService>();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}