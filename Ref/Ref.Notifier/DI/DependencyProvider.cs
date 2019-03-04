using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Ref.Data.Components;
using Ref.Data.Db;
using Ref.Data.Repositories;
using Ref.Notifier.Core;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using System;

namespace Ref.Notifier.DI
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

            services.AddTransient<IEmailProvider>(
                s => new EmailProvider(
                    configurationRoot["notifications:email:host"],
                    configurationRoot["notifications:email:apikey"])
                );

            services.AddTransient<IEmailNotification, EmailNotification>();
            #endregion

            #region Repositories
            services.AddTransient<IOfferFilterRepository, OfferFilterRepository>();

            services.AddTransient<IMailReport, MailReport>();
            #endregion

            #region App
            services.AddTransient<IAppNotifierProvider>(
            s => new AppNotifierProvider(
                configurationRoot["app:address"],
                configurationRoot["app:sender"],
                configurationRoot["app:replyto"],
                configurationRoot["app:pausetime"],
                configurationRoot["app:adminnotification"],
                configurationRoot["app:successtries"],
                appId)
            );
            #endregion

            services.AddTransient<NotifierService>();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}