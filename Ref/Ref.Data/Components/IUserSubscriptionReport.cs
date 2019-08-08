using Dapper;
using Ref.Data.Db;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ref.Data.Components
{
    public interface IUserSubscriptionReport
    {
        Task<IEnumerable<UserSubscription>> GetAllActiveAsync();
        Task<IEnumerable<UserSubscriptionFilter>> GetAllActiveFiltersAsync();
    }

    public class UserSubscriptionReport : IUserSubscriptionReport
    {
        private readonly IDbAccess _dbAccess;

        public UserSubscriptionReport(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<IEnumerable<UserSubscription>> GetAllActiveAsync()
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<UserSubscription>(
                    @"SELECT F.Id, F.UserId, F.Property, F.Deal, F.Market, F.CityId, F.FlatAreaFrom, F.FlatAreaTo, F.PriceFrom, F.PriceTo, F.Name, F.Notification, F.LastCheckedAt, F.ShouldContain, F.ShouldNotContain, U.IsActive as IsUserActive, F.PricePerMeterFrom, F.PricePerMeterTo, U.Subscription, U.RegisteredAt, F.DistrictId, F.AllowFromAgency 
                        FROM Filters as F INNER JOIN Users U on F.UserId = U.Id WHERE U.IsActive = 1 AND F.Notification <> 100");
            }
        }

        public async Task<IEnumerable<UserSubscriptionFilter>> GetAllActiveFiltersAsync()
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<UserSubscriptionFilter>(
                    @"SELECT F.Property, F.Deal, F.Market, F.CityId, C.NameRaw as City, C.GtCodeSale as GumtreeCitySale, F.FlatAreaFrom, F.FlatAreaTo, F.PriceFrom, F.PriceTo, F.DistrictId, D.NameRaw as District, D.OlxId, D.AdrId, D.GtCodeSale as GumtreeDistrictSale, U.Subscription, U.RegisteredAt, F.AllowPrivate, F.AllowFromAgency 
                        FROM Filters as F 
                        INNER JOIN Users U on F.UserId = U.Id 
                        INNER JOIN Cities C on F.CityId = C.Id 
						LEFT JOIN Districts D on F.DistrictId = D.Id
                        WHERE U.IsActive = 1 AND F.Notification <> 100");
            }
        }
    }
}