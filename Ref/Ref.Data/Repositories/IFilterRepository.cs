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
                    @"SELECT Id, UserId, Type, Deal, Market, Location, FlatAreaFrom, FlatAreaTo, PriceFrom, PriceTo, Newest FROM Filters");
            }
        }

        public async Task<IQueryable<Filter>> FindByAsync(Expression<Func<Filter, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<Filter>(
                    @"SELECT Id, UserId, Type, Deal, Market, Location, FlatAreaFrom, FlatAreaTo, PriceFrom, PriceTo, Newest FROM Filters")).AsQueryable();

                return result.Where(predicate);
            }
        }

        public async Task<int> CreateAsync(Filter filter)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"INSERT INTO Filters (UserId, Type, Deal, Market, Location, FlatAreaFrom, FlatAreaTo, PriceFrom, PriceTo, Newest)
                        VALUES(@UserId, @Type, @Deal, @Market, @Location, @FlatAreaFrom, @FlatAreaTo, @PriceFrom, @PriceTo, @Newest);
                    SELECT CAST(SCOPE_IDENTITY() as int)",
                    new
                    {
                        filter.UserId,
                        filter.Type,
                        filter.Deal,
                        filter.Market,
                        filter.Location,
                        filter.FlatAreaFrom,
                        filter.FlatAreaTo,
                        filter.PriceFrom,
                        filter.PriceTo,
                        filter.Newest
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
                    @"UPDATE Filters SET Type = @Type, Deal = @Deal, Market = @Market, Location = @Location, FlatAreaFrom = @FlatAreaFrom, FlatAreaTo = @FlatAreaTo, PriceFrom = @PriceFrom, PriceTo = @PriceTo, Newest = @Newest
                        WHERE Id = @Id AND UserId = @UserId",
                    new
                    {
                        filter.Id,
                        filter.UserId,
                        filter.Type,
                        filter.Deal,
                        filter.Market,
                        filter.Location,
                        filter.FlatAreaFrom,
                        filter.FlatAreaTo,
                        filter.PriceFrom,
                        filter.PriceTo,
                        filter.Newest
                    });
            }
        }

        public async Task<Filter> GetAsync(int filterId, int userId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryFirstOrDefaultAsync<Filter>(
                    @"SELECT Id, UserId, Type, Deal, Market, Location, FlatAreaFrom, FlatAreaTo, PriceFrom, PriceTo, Newest FROM Filters 
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