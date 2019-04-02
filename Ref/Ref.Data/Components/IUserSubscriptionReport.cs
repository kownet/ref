using Dapper;
using Ref.Data.Db;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ref.Data.Components
{
    public interface IUserSubscriptionReport
    {
        Task<IEnumerable<UserSubscription>> GetAllActiveAsync();
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
                    @"SELECT F.Id, F.UserId, F.Property, F.Deal, F.Market, F.CityId, F.FlatAreaFrom, F.FlatAreaTo, F.PriceFrom, F.PriceTo, F.Name, F.Notification, F.LastCheckedAt, F.ShouldContain, F.ShouldNotContain, U.IsActive as IsUserActive
                        FROM Filters as F INNER JOIN Users U on F.UserId = U.Id WHERE U.IsActive = 1 AND F.Notification <> 100");
            }
        }
    }
}