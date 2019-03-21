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
    public interface IUserRepository : IRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetAsync(int userId);
        Task<int> CreateAsync(User user);
        Task<int> UpdateAsync(User user);
        Task<int> DeleteAsync(int userId);
        Task<IQueryable<User>> FindByAsync(Expression<Func<User, bool>> predicate);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDbAccess _dbAccess;

        public UserRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<int> CreateAsync(User user)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"INSERT INTO Users (Email, PasswordHash, PasswordSalt, Role, RegisteredAt, Guid) VALUES(@Email, @PasswordHash, @PasswordSalt, @Role, @RegisteredAt, @Guid);
                    SELECT CAST(SCOPE_IDENTITY() as int)",
                    new
                    {
                        user.Email,
                        user.PasswordHash,
                        user.PasswordSalt,
                        user.Role,
                        RegisteredAt = DateTime.Now,
                        Guid = Guid.NewGuid().ToString().ToUpperInvariant()
                    });
            }
        }

        public async Task<int> DeleteAsync(int userId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"DELETE FROM Users WHERE Id = @UserId",
                    new
                    {
                        UserId = userId
                    });
            }
        }

        public async Task<IQueryable<User>> FindByAsync(Expression<Func<User, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<User>(
                    @"SELECT Id, Email, PasswordHash, PasswordSalt, Role, Guid, RegisteredAt FROM Users")).AsQueryable();

                return result.Where(predicate);
            }
        }

        public async Task<User> GetAsync(int userId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QuerySingleOrDefaultAsync<User>(
                    @"SELECT Id, Email, PasswordHash, PasswordSalt, Role, Guid FROM Users WHERE Id = @Id",
                    new
                    {
                        Id = userId
                    });
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<User>(@"SELECT Id, Email, Guid FROM Users");
            }
        }

        public async Task<int> UpdateAsync(User user)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"UPDATE Users SET Email = @Email, PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt, Role = @Role, Guid = @Guid WHERE Id = @Id",
                    new
                    {
                        user.Email,
                        user.PasswordHash,
                        user.PasswordSalt,
                        user.Id,
                        user.Role,
                        user.Guid
                    });
            }
        }
    }
}