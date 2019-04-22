using Ref.Data.Models;
using Ref.Data.Repositories.Standalone;
using Ref.Sites.Helpers;

namespace Ref.Sites.Scrapper
{
    public interface ISiteToScrapp
    {
        SiteResponse Search(SearchFilter filter);
        ScrappResponse Scrapp(City city, DealType dealType, District district);
    }
}