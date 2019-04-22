using Dapper;
using Ref.Data.Db;
using Ref.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ref.Data.Components
{
    public interface ICitiesReport
    {
        Task<IEnumerable<City>> GetAllCitiesForActiveUsersAsync();
        Task<IEnumerable<District>> GetAllDistrictsForActiveCityAsync(int cityId);
    }

    public class CitiesReport : ICitiesReport
    {
        private readonly IDbAccess _dbAccess;

        public CitiesReport(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<IEnumerable<City>> GetAllCitiesForActiveUsersAsync()
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<City>(
                    @"select distinct C.Id, C.Name, C.NameRaw, C.GtCodeSale, C.GtCodeRent, C.HasDistricts from Cities C
                        inner join Filters F on C.Id = F.CityId
                        inner join Users U on U.Id = F.UserId
                        where U.IsActive = 1 and F.Notification <> 100");
            }
        }

        public async Task<IEnumerable<District>> GetAllDistrictsForActiveCityAsync(int cityId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<District>(
                    @"select distinct D.Id, D.Name, D.NameRaw, D.GtCodeRent, D.GtCodeSale, D.CityId, D.OtoDomId, D.OlxId, D.AdrId from Districts D
						inner join Filters F on D.Id = F.DistrictId
						where D.CityId = @CityId", new { CityId = cityId });
            }
        }
    }
}