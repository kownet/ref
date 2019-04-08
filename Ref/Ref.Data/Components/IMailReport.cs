using Dapper;
using Ref.Data.Db;
using Ref.Shared.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ref.Data.Components
{
    public interface IMailReport
    {
        Task<IEnumerable<MailReportFilter>> GetAllAsync();
        Task<IEnumerable<MailReportOffer>> GetAllOffersForFilterAsync(int filterId);
        Task<int> UpdateFiltersAsSentAsync(IEnumerable<int> filterIds);
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
                    SELECT U.Email, U.Id as UserId, F.Name as Filter, F.Id as FilterId, U.Guid as Token FROM Users U
                    INNER JOIN Filters F ON F.UserId = U.Id");
            }
        }

        public async Task<IEnumerable<MailReportOffer>> GetAllOffersForFilterAsync(int filterId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<MailReportOffer>(@"
                    SELECT O.Header, O.Url, O.Price, O.Site, O.Area FROM Offers O
                    INNER JOIN OfferFilters FO ON FO.OfferId = O.Id
                    WHERE FO.FilterId = @FilterId AND FO.Sent = 0",
                    new
                    {
                        FilterId = filterId
                    });
            }
        }

        public async Task<int> UpdateFiltersAsSentAsync(IEnumerable<int> filterIds)
        {
            var result = 0;

            using (var c = _dbAccess.Connection)
            {
                var chunked = filterIds.Chunk(1000);

                foreach (var chunk in chunked)
                {
                    result = await c.ExecuteAsync(
                            @"UPDATE OfferFilters SET Sent = 1 WHERE FilterId IN @Ids",
                            new
                            {
                                Ids = chunk
                            });
                }
            }

            return result;
        }
    }
}