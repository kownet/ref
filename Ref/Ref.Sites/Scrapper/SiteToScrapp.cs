using HtmlAgilityPack;
using Ref.Data.Models;
using Ref.Shared.Providers;
using Ref.Sites.Pages;
using Ref.Sites.QueryStrings;
using System;

namespace Ref.Sites.Scrapper
{
    public abstract class SiteToScrapp
    {
        protected readonly IAppProvider AppProvider;
        protected readonly Func<SiteType, IPages> PageProvider;
        protected readonly Func<SiteType, IQueryString> QueryStringProvider;

        protected readonly HtmlWeb Scrapper;

        public SiteToScrapp(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider)
        {
            AppProvider = appProvider;
            PageProvider = pageProvider;
            QueryStringProvider = queryStringProvider;

            Scrapper = new HtmlWeb();
        }
    }
}