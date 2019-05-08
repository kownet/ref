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
    public interface ICitiesRepository : IRepository
    {
        Task<IEnumerable<City>> GetAllAsync();
        Task<IQueryable<City>> FindByAsync(Expression<Func<City, bool>> predicate);
    }

    public class CitiesRepository : ICitiesRepository
    {
        private readonly IDbAccess _dbAccess;

        public CitiesRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<IQueryable<City>> FindByAsync(Expression<Func<City, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<City>(
                    @"SELECT Id, Name, NameRaw, GtCodeSale, GtCodeRent, HasDistricts FROM Cities")).AsQueryable();

                return result.Where(predicate);
            }
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<City>(
                    @"SELECT Id, Name, NameRaw, GtCodeSale, GtCodeRent, HasDistricts FROM Cities");
            }
        }
    }
}