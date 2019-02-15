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
        Task<IEnumerable<Filter>> GetAll();
        Task<IQueryable<Filter>> FindByAsync(Expression<Func<Filter, bool>> predicate);
    }

    public class FilterRepository : IFilterRepository
    {
        private readonly IDbAccess _dbAccess;

        public FilterRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<IEnumerable<Filter>> GetAll()
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
    }
}