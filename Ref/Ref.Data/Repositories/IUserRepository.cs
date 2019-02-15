using Dapper;
using Newtonsoft.Json;
using Ref.Data.Db;
using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ref.Data.Repositories
{
    public interface IUserRepository : IRepository
    {
        IEnumerable<User> GetAll();
        Task<User> GetAsync(int userId);
        Task<int> Create(User user);
        void Update(User user, string password = null);
        void Delete(int id);
        Task<IQueryable<User>> FindByAsync(Expression<Func<User, bool>> predicate);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDbAccess _dbAccess;

        public UserRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<int> Create(User user)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"INSERT INTO Users (Email, PasswordHash, PasswordSalt) VALUES(@Email, @PasswordHash, @PasswordSalt);
                    SELECT CAST(SCOPE_IDENTITY() as int)",
                    new
                    {
                        user.Email,
                        user.PasswordHash,
                        user.PasswordSalt
                    });
            }
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<User>> FindByAsync(Expression<Func<User, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<User>(
                    @"SELECT Id, Email, PasswordHash, PasswordSalt FROM Users")).AsQueryable();

                return result.Where(predicate);
            }
        }

        public async Task<User> GetAsync(int userId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QuerySingleOrDefaultAsync<User>(
                    @"SELECT Id, Email, PasswordHash, PasswordSalt FROM Users WHERE Id = @Id",
                    new
                    {
                        Id = userId
                    });
            }
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
            //using (var c = _dbAccess.GetConnection())
            //{
            //    return await c.QueryAsync<User>(@"SELECT Id, Email, PasswordHash, PasswordSalt FROM Users");
            //}
        }

        public void Update(User user, string password = null)
        {
            throw new NotImplementedException();
        }
    }

    public class UserJsonRepository : IUserRepository
    {
        private readonly IStorageProvider _storageProvider;

        public UserJsonRepository(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public async Task<int> Create(User user)
        {
            return 0;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<User>> FindByAsync(Expression<Func<User, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public User Get(int userId)
            => GetAll().FirstOrDefault(c => c.Id == userId);

        public IEnumerable<User> GetAll()
        {
            var result = new List<User>();

            var files = Directory.GetFiles(_storageProvider.ClientsPath(), "*.json", SearchOption.TopDirectoryOnly);

            if (files.AnyAndNotNull())
            {
                foreach (var file in files)
                {
                    var clientRaw = File.ReadAllText(file);

                    if (!string.IsNullOrWhiteSpace(clientRaw))
                    {
                        var clientEntity = JsonConvert.DeserializeObject<User>(clientRaw);

                        if (clientEntity != null)
                        {
                            result.Add(clientEntity);
                        }
                    }
                }
            }

            return result;
        }

        public Task<User> GetAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public void Update(User user, string password = null)
        {
            throw new System.NotImplementedException();
        }
    }
}