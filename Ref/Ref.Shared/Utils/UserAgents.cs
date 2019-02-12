using System;
using System.Collections.Generic;

namespace Ref.Shared.Utils
{
    public class UserAgents
    {
        private static Random R = new Random();

        public string GetRandom()
        {
            var list = new List<string>
            {
                "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_1) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 41.0.2227.1 Safari / 537.36",
                "Mozilla / 5.0(X11; Linux x86_64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 41.0.2227.0 Safari / 537.36",
                "Mozilla / 5.0(Windows NT 6.1; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 41.0.2227.0 Safari / 537.36",
                "Mozilla / 5.0(Windows NT 6.3; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 41.0.2226.0 Safari / 537.36",
                "Mozilla / 5.0(Windows NT 6.4; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 41.0.2225.0 Safari / 537.36",
                "Mozilla / 5.0(Windows NT 6.3; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 41.0.2225.0 Safari / 537.36",
                "Mozilla/5.0 (Windows NT 6.1; WOW64; rv: 40.0) Gecko / 20100101 Firefox / 40.1",
                "Mozilla / 5.0(Windows NT 6.3; rv: 36.0) Gecko / 20100101 Firefox / 36.0",
                "Mozilla / 5.0(Macintosh; Intel Mac OS X 10_10; rv: 33.0) Gecko / 20100101 Firefox / 33.0",
                "Mozilla / 5.0(X11; Linux i586; rv: 31.0) Gecko / 20100101 Firefox / 31.0",
                "Mozilla / 5.0(Windows NT 6.1; WOW64; rv: 31.0) Gecko / 20130401 Firefox / 31.0",
                "Mozilla / 5.0(Windows NT 5.1; rv: 31.0) Gecko / 20100101 Firefox / 31.0",
                "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident / 6.0)",
                "Mozilla / 5.0(compatible; MSIE 10.0; Windows NT 6.1; Trident / 6.0)",
                "Mozilla / 5.0(compatible; MSIE 10.0; Windows NT 6.1; Trident / 5.0)",
                "Mozilla / 5.0(compatible; MSIE 10.0; Windows NT 6.1; Trident / 4.0; InfoPath.2; SV1; .NET CLR 2.0.50727; WOW64)",
                "Mozilla / 5.0(compatible; MSIE 10.0; Macintosh; Intel Mac OS X 10_7_3; Trident / 6.0)",
                "Opera/9.80 (Windows NT 6.1; U; es-ES) Presto/2.9.181 Version/12.00",
                "Opera/9.80 (Windows NT 5.1; U; zh - sg) Presto / 2.9.181 Version / 12.00",
                "Opera / 12.0(Windows NT 5.2; U; en)Presto / 22.9.168 Version / 12.00",
                "Opera / 12.0(Windows NT 5.1; U; en)Presto / 22.9.168 Version / 12.00"
            };

            return list[R.Next(0, list.Count)];
        }
    }
}