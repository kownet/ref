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
    public interface ISiteRepository : IRepository
    {
        Task<IEnumerable<Site>> GetAllAsync();
        Task<Site> GetAsync(int siteId);
        Task<int> CreateAsync(Site site);
        Task<int> UpdateAsync(Site site);
        Task<int> DeleteAsync(int siteId);
        Task<IQueryable<Site>> FindByAsync(Expression<Func<Site, bool>> predicate);
    }

    public class SiteRepository : ISiteRepository
    {
        private readonly IDbAccess _dbAccess;

        public SiteRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<int> CreateAsync(Site site)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"INSERT INTO Sites (Type, Name, IsActive) VALUES(@Type, @Name, @IsActive);
                    SELECT CAST(SCOPE_IDENTITY() as int)",
                    new
                    {
                        site.Type,
                        site.Name,
                        site.IsActive
                    });
            }
        }

        public async Task<int> DeleteAsync(int siteId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"DELETE FROM Sites WHERE Id = @SiteId",
                    new
                    {
                        SiteId = siteId
                    });
            }
        }

        public async Task<IQueryable<Site>> FindByAsync(Expression<Func<Site, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<Site>(
                    @"SELECT Id, Type, Name, IsActive FROM Sites")).AsQueryable();

                return result.Where(predicate);
            }
        }

        public async Task<IEnumerable<Site>> GetAllAsync()
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<Site>(@"SELECT Id, Type, Name, IsActive FROM Sites");
            }
        }

        public async Task<Site> GetAsync(int siteId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QuerySingleOrDefaultAsync<Site>(
                    @"SELECT Id, Type, Name, IsActive FROM Sites WHERE Id = @Id",
                    new
                    {
                        Id = siteId
                    });
            }
        }

        public async Task<int> UpdateAsync(Site site)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"UPDATE Sites SET Type = @Type, Name = @Name, IsActive = @IsActive WHERE Id = @Id",
                    new
                    {
                        site.Id,
                        site.Type,
                        site.Name,
                        site.IsActive
                    });
            }
        }
    }
}