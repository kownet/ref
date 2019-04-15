using Dapper;
using Ref.Data.Db;
using Ref.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ref.Data.Repositories
{
    public interface ICitiesRepository : IRepository
    {
        Task<IEnumerable<City>> GetAllAsync();
    }

    public class CitiesRepository : ICitiesRepository
    {
        private readonly IDbAccess _dbAccess;

        public CitiesRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
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