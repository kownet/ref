using HtmlAgilityPack;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using Ref.Shared.Utils;
using Ref.Sites.Helpers;
using Ref.Sites.Pages;
using Ref.Sites.QueryStrings;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Ref.Sites.Scrapper
{
    public abstract class SiteToScrapp
    {
        protected readonly IAppProvider AppProvider;
        protected readonly Func<SiteType, IPages> PageProvider;
        protected readonly Func<SiteType, IQueryString> QueryStringProvider;
        protected readonly IDistrictRepository DistrictRepository;

        public SiteToScrapp(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider,
            IDistrictRepository districtRepository)
        {
            AppProvider = appProvider;
            PageProvider = pageProvider;
            QueryStringProvider = queryStringProvider;
            DistrictRepository = districtRepository;
        }

        protected SiteToScrappResponse ScrapThis(string url, string specialEncoding = "")
        {
            try
            {
                Thread.Sleep(AppProvider.PauseTime());

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Timeout = AppProvider.Timeout();
                httpWebRequest.UserAgent = new UserAgents().GetRandom();

                if (!string.IsNullOrWhiteSpace(AppProvider.Address()))
                {
                    httpWebRequest.Referer = AppProvider.Address();
                }

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

                                    return new SiteToScrappResponse { HtmlNode = doc.DocumentNode };
                                }
                            }
                            else
                            {
                                using (var reader = new StreamReader(responseStream, Encoding.GetEncoding(specialEncoding)))
                                {
                                    var htmlstring = reader.ReadToEnd();

                                    var doc = new HtmlDocument();

                                    doc.LoadHtml(htmlstring);

                                    return new SiteToScrappResponse { HtmlNode = doc.DocumentNode };
                                }
                            }
                        }
                    }
                    else
                    {
                        return new SiteToScrappResponse { ExceptionMessage = "Response status not OK" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new SiteToScrappResponse
                {
                    ExceptionAccured = true,
                    ExceptionMessage = ex.GetFullMessage()
                };
            }
        }
    }
}