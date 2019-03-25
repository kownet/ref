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
    public interface IAdminInfosRepository : IRepository
    {
        Task<IEnumerable<AdminInfo>> GetAllAsync();
        Task<AdminInfo> GetAsync(int infoId);
        Task<int> CreateAsync(AdminInfo info);
        Task<int> UpdateAsync(AdminInfo info);
        Task<int> DeleteAsync(int infoId);
        Task<IQueryable<AdminInfo>> FindByAsync(Expression<Func<AdminInfo, bool>> predicate);
    }

    public class AdminInfosRepository : IAdminInfosRepository
    {
        private readonly IDbAccess _dbAccess;

        public AdminInfosRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<int> CreateAsync(AdminInfo info)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"INSERT INTO AdminInfos (Text, IsActive, DateAdded) VALUES(@Text, @IsActive, @DateAdded);
                    SELECT CAST(SCOPE_IDENTITY() as int)",
                    new
                    {
                        info.Text,
                        info.IsActive,
                        DateAdded = DateTime.Now
                    });
            }
        }

        public async Task<int> DeleteAsync(int infoId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"DELETE FROM AdminInfos WHERE Id = @Id",
                    new
                    {
                        Id = infoId
                    });
            }
        }

        public async Task<IQueryable<AdminInfo>> FindByAsync(Expression<Func<AdminInfo, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<AdminInfo>(
                    @"SELECT Id, Text, IsActive, DateAdded FROM AdminInfos")).AsQueryable();

                return result.Where(predicate);
            }
        }

        public async Task<IEnumerable<AdminInfo>> GetAllAsync()
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<AdminInfo>(@"SELECT Id, Text, IsActive, DateAdded FROM AdminInfos");
            }
        }

        public async Task<AdminInfo> GetAsync(int infoId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QuerySingleOrDefaultAsync<AdminInfo>(
                    @"SELECT Id, Text, IsActive, DateAdded FROM AdminInfos WHERE Id = @Id",
                    new
                    {
                        Id = infoId
                    });
            }
        }

        public async Task<int> UpdateAsync(AdminInfo info)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"UPDATE AdminInfos SET Text = @Text, IsActive = @IsActive WHERE Id = @Id",
                    new
                    {
                        info.Text,
                        info.IsActive,
                        info.Id
                    });
            }
        }
    }
}