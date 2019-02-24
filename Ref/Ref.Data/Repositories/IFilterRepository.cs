using Dapper;
using Ref.Data.Db;
using Ref.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ref.Data.Repositories
{
    public interface IFilterRepository : IRepository
    {
        Task<IEnumerable<Filter>> GetAllAsync();
        Task<IQueryable<Filter>> FindByAsync(Expression<Func<Filter, bool>> predicate);
        Task<int> CreateAsync(Filter filter);
        Task<int> DeleteAsync(int filterId, int userId);
        Task<int> UpdateAsync(Filter filter);
        Task<Filter> GetAsync(int filterId, int userId);
    }

    public class FilterRepository : IFilterRepository
    {
        private readonly IDbAccess _dbAccess;

        public FilterRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<IEnumerable<Filter>> GetAllAsync()
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<Filter>(
                    @"SELECT Id, UserId, Property, Deal, Market, CityId, FlatAreaFrom, FlatAreaTo, PriceFrom, PriceTo, Name, Notification, LastCheckedAt FROM Filters");
            }
        }

        public async Task<IQueryable<Filter>> FindByAsync(Expression<Func<Filter, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<Filter>(
                    @"SELECT Id, UserId, Property, Deal, Market, CityId, FlatAreaFrom, FlatAreaTo, PriceFrom, PriceTo, Name, Notification, LastCheckedAt FROM Filters")).AsQueryable();

                return result.Where(predicate);
            }
        }

        public async Task<int> CreateAsync(Filter filter)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"INSERT INTO Filters (UserId, Property, Deal, Market, FlatAreaFrom, FlatAreaTo, PriceFrom, PriceTo, CityId, Name, Notification)
                        VALUES(@UserId, @Property, @Deal, @Market, @FlatAreaFrom, @FlatAreaTo, @PriceFrom, @PriceTo, @CityId, @Name, @Notification);
                    SELECT CAST(SCOPE_IDENTITY() as int)",
                    new
                    {
                        filter.UserId,
                        filter.Property,
                        filter.Deal,
                        filter.Market,
                        filter.FlatAreaFrom,
                        filter.FlatAreaTo,
                        filter.PriceFrom,
                        filter.PriceTo,
                        filter.CityId,
                        filter.Name,
                        filter.Notification
                    });
            }
        }

        public async Task<int> DeleteAsync(int filterId, int userId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"DELETE FROM Filters WHERE Id = @Id AND UserId = @UserId",
                    new
                    {
                        Id = filterId,
                        UserId = userId
                    });
            }
        }

        public async Task<int> UpdateAsync(Filter filter)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"UPDATE Filters SET Property = @Property, Deal = @Deal, Market = @Market, CityId = @CityId, FlatAreaFrom = @FlatAreaFrom, FlatAreaTo = @FlatAreaTo, PriceFrom = @PriceFrom, PriceTo = @PriceTo, Name = @Name 
                        WHERE Id = @Id AND UserId = @UserId",
                    new
                    {
                        filter.Id,
                        filter.UserId,
                        filter.Property,
                        filter.Deal,
                        filter.Market,
                        filter.CityId,
                        filter.FlatAreaFrom,
                        filter.FlatAreaTo,
                        filter.PriceFrom,
                        filter.PriceTo,
                        filter.Name
                    });
            }
        }

        public async Task<Filter> GetAsync(int filterId, int userId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryFirstOrDefaultAsync<Filter>(
                    @"SELECT Id, UserId, Type, Deal, Market, Location, FlatAreaFrom, FlatAreaTo, PriceFrom, PriceTo, Name, Notification, LastCheckedAt FROM Filters 
                        WHERE Id = @Id AND UserId = @UserId",
                    new
                    {
                        Id = filterId,
                        UserId = userId
                    });
            }
        }
    }
}