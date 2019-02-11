using HtmlAgilityPack;
using Ref.Data.Models;
using Ref.Shared.Providers;
using Ref.Sites.Pages;
using Ref.Sites.QueryStrings;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Ref.Sites.Scrapper
{
    public abstract class SiteToScrapp
    {
        protected readonly IAppProvider AppProvider;
        protected readonly Func<SiteType, IPages> PageProvider;
        protected readonly Func<SiteType, IQueryString> QueryStringProvider;

        public SiteToScrapp(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider)
        {
            AppProvider = appProvider;
            PageProvider = pageProvider;
            QueryStringProvider = queryStringProvider;
        }

        protected HtmlNode ScrapThis(string url, int timeout = 30000, string specialEncoding = "")
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = timeout;

            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var responseStream = httpWebResponse.GetResponseStream())
                    {
                        if (string.IsNullOrWhiteSpace(specialEncoding))
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                var htmlstring = reader.ReadToEnd();

                                var doc = new HtmlDocument();

                                doc.LoadHtml(htmlstring);

                                return doc.DocumentNode;
                            }
                        }
                        else
                        {
                            using (var reader = new StreamReader(responseStream, Encoding.GetEncoding(specialEncoding)))
                            {
                                var htmlstring = reader.ReadToEnd();

                                var doc = new HtmlDocument();

                                doc.LoadHtml(htmlstring);

                                return doc.DocumentNode;
                            }
                        }
                    }
                }
                else
                    throw new Exception($"Cannot get the site to scrapp: '{url}'.");
            }
        }
    }
}