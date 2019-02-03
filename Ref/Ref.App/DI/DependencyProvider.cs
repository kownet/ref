﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Ref.App.Core;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Sites;
using System;
using System.Collections.Generic;

namespace Ref.App.DI
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
            services.AddTransient<IStorageProvider>(
                s => new StorageProvider(
                    configurationRoot["storages:file_json:result"],
                    configurationRoot["storages:file_json:clients"])
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
            services.AddTransient<IClientRepository, ClientJsonRepository>();
            services.AddTransient<IAdRepository, AdJsonRepository>();
            #endregion

            #region Sites
            services.AddTransient<OtoDomSite>();
            services.AddTransient<OlxSite>();
            services.AddTransient<AdresowoSite>();
            services.AddTransient<DomiportaSite>();
            services.AddTransient<GratkaSite>();
            services.AddTransient<MorizonSite>();
            services.AddTransient<GumtreeSite>();

            services.AddTransient<Func<SiteType, ISite>>(sp => key =>
            {
                switch (key)
                {
                    case SiteType.OtoDom:
                        return sp.GetService<OtoDomSite>();
                    case SiteType.Olx:
                        return sp.GetService<OlxSite>();
                    case SiteType.Adresowo:
                        return sp.GetService<AdresowoSite>();
                    case SiteType.DomiPorta:
                        return sp.GetService<DomiportaSite>();
                    case SiteType.Gratka:
                        return sp.GetService<GratkaSite>();
                    case SiteType.Morizon:
                        return sp.GetService<MorizonSite>();
                    case SiteType.Gumtree:
                        return sp.GetService<GumtreeSite>();
                    default:
                        throw new KeyNotFoundException();
                }
            });
            #endregion

            #region App
            services.AddTransient<IAppProvider>(
                s => new AppProvider(
                    configurationRoot["app:sender"],
                    configurationRoot["app:replyto"],
                    configurationRoot["app:binpath"],
                    configurationRoot["app:pausetime"],
                    configurationRoot["app:sites"])
                );
            #endregion

            services.AddTransient<RefService>();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}