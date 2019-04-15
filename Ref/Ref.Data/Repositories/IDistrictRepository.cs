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
    public interface IDistrictRepository : IRepository
    {
        Task<IEnumerable<District>> GetAllAsync();
        Task<IQueryable<District>> FindByAsync(Expression<Func<District, bool>> predicate);
    }

    public class DistrictRepository : IDistrictRepository
    {
        private readonly IDbAccess _dbAccess;

        public DistrictRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<IQueryable<District>> FindByAsync(Expression<Func<District, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<District>(
                    @"SELECT Id, CityId, Name, NameRaw, GtCodeSale, GtCodeRent FROM Districts")).AsQueryable();

                return result.Where(predicate);
            }
        }

        public async Task<IEnumerable<District>> GetAllAsync()
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<District>(
                    @"SELECT Id, CityId, Name, NameRaw, GtCodeSale, GtCodeRent FROM Districts");
            }
        }
    }
}