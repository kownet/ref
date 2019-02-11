using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Ref.App.Core;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Sites.Pages;
using Ref.Sites.QueryStrings;
using Ref.Sites.Scrapper;
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

            #region QueryStrings
            services.AddTransient<OtoDomQueryString>();
            services.AddTransient<OlxQueryString>();
            services.AddTransient<AdresowoQueryString>();
            services.AddTransient<DomiportaQueryString>();
            services.AddTransient<GratkaQueryString>();
            services.AddTransient<MorizonQueryString>();
            services.AddTransient<GumtreeQueryString>();

            services.AddTransient<Func<SiteType, IQueryString>>(sp => key =>
            {
                switch (key)
                {
                    case SiteType.OtoDom:
                        return sp.GetService<OtoDomQueryString>();
                    case SiteType.Olx:
                        return sp.GetService<OlxQueryString>();
                    case SiteType.Adresowo:
                        return sp.GetService<AdresowoQueryString>();
                    case SiteType.DomiPorta:
                        return sp.GetService<DomiportaQueryString>();
                    case SiteType.Gratka:
                        return sp.GetService<GratkaQueryString>();
                    case SiteType.Morizon:
                        return sp.GetService<MorizonQueryString>();
                    case SiteType.Gumtree:
                        return sp.GetService<GumtreeQueryString>();
                    default:
                        throw new KeyNotFoundException();
                }
            });
            #endregion

            #region Pages
            services.AddTransient<OtoDomPages>();
            services.AddTransient<OlxPages>();
            services.AddTransient<AdresowoPages>();
            services.AddTransient<DomiportaPages>();
            services.AddTransient<GratkaPages>();
            services.AddTransient<MorizonPages>();
            services.AddTransient<GumtreePages>();

            services.AddTransient<Func<SiteType, IPages>>(sp => key =>
            {
                switch (key)
                {
                    case SiteType.OtoDom:
                        return sp.GetService<OtoDomPages>();
                    case SiteType.Olx:
                        return sp.GetService<OlxPages>();
                    case SiteType.Adresowo:
                        return sp.GetService<AdresowoPages>();
                    case SiteType.DomiPorta:
                        return sp.GetService<DomiportaPages>();
                    case SiteType.Gratka:
                        return sp.GetService<GratkaPages>();
                    case SiteType.Morizon:
                        return sp.GetService<MorizonPages>();
                    case SiteType.Gumtree:
                        return sp.GetService<GumtreePages>();
                    default:
                        throw new KeyNotFoundException();
                }
            });
            #endregion

            #region Sites
            services.AddTransient<OtoDom>();
            services.AddTransient<Olx>();
            services.AddTransient<Adresowo>();
            services.AddTransient<Domiporta>();
            services.AddTransient<Gratka>();
            services.AddTransient<Morizon>();
            services.AddTransient<Gumtree>();

            services.AddTransient<Func<SiteType, ISiteToScrapp>>(sp => key =>
            {
                switch (key)
                {
                    case SiteType.OtoDom:
                        return sp.GetService<OtoDom>();
                    case SiteType.Olx:
                        return sp.GetService<Olx>();
                    case SiteType.Adresowo:
                        return sp.GetService<Adresowo>();
                    case SiteType.DomiPorta:
                        return sp.GetService<Domiporta>();
                    case SiteType.Gratka:
                        return sp.GetService<Gratka>();
                    case SiteType.Morizon:
                        return sp.GetService<Morizon>();
                    case SiteType.Gumtree:
                        return sp.GetService<Gumtree>();
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
                configurationRoot["app:bcc"],
                configurationRoot["app:pausetime"],
                configurationRoot["app:sites"],
                appId,
                configurationRoot["app:adminnotification"],
                configurationRoot["app:successtries"])
            );
            #endregion

            services.AddTransient<RefService>();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}