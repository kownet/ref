using Dapper;
using Ref.Data.Db;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ref.Data.Components
{
    public interface IMailReport
    {
        Task<IEnumerable<MailReportFilter>> GetAllAsync();
        Task<IEnumerable<MailReportOffer>> GetAllOffersForFilterAsync(int filterId);
    }

    public class MailReport : IMailReport
    {
        private readonly IDbAccess _dbAccess;

        public MailReport(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<IEnumerable<MailReportFilter>> GetAllAsync()
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<MailReportFilter>(@"
                    SELECT U.Email, U.Id as UserId, F.Name as Filter, F.Id as FilterId FROM Users U
                    INNER JOIN Filters F ON F.UserId = U.Id");
            }
        }

        public async Task<IEnumerable<MailReportOffer>> GetAllOffersForFilterAsync(int filterId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<MailReportOffer>(@"
                    SELECT O.Header, O.Url, O.Price, O.SiteType FROM Offers O
                    INNER JOIN OfferFilters FO ON FO.OfferId = O.Id
                    WHERE FO.FilterId = @FilterId AND FO.Sent = 0",
                    new
                    {
                        FilterId = filterId
                    });
            }
        }
    }
}