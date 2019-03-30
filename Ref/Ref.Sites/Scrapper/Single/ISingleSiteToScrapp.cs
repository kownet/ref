using Ref.Data.Models;
using Ref.Sites.Helpers;

namespace Ref.Sites.Scrapper.Single
{
    public interface ISingleSiteToScrapp
    {
        SingleScrappResponse SingleScrapp(Offer offer);
    }
}