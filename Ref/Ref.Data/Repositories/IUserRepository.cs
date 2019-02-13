using Newtonsoft.Json;
using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ref.Data.Repositories
{
    public interface IUserRepository : IRepository
    {
        IEnumerable<User> GetAll();
        User Get(int userId);
        User Create(User user);
        void Update(User user, string password = null);
        void Delete(int id);
    }

    public class UserInMemoryRepository : IUserRepository
    {
        private List<User> _users = new List<User>
        {
            new User { Id = 1, Email = "test",
                PasswordHash = "B5-7E-BA-D2-C9-5F-58-27-4E-7C-F6-8B-ED-13-28-78-48-87-BA-E2-44-03-D2-54-4D-1A-67-29-56-8D-3F-1C-5D-D8-9B-CD-C5-8F-4B-A2-B6-9B-3F-F5-26-62-89-5A-30-37-02-26-5F-23-2A-98-24-26-3B-2E-C2-03-CF-EB",
                PasswordSalt = "59-94-3E-28-F3-09-E6-25-16-9D-D2-25-AC-3F-10-19-05-17-FE-CA-A8-47-49-1C-DB-C2-DF-80-35-01-70-BA-3A-5D-75-78-51-2E-13-5E-4A-EC-F7-DF-A5-ED-4A-4D-D4-3E-C7-38-A4-7B-80-BE-AA-4C-9D-6F-92-59-A7-4B-29-66-F5-65-EE-13-CA-F7-AE-9E-68-51-F6-CA-85-BB-3C-D6-F2-39-3E-46-70-CB-91-D2-64-81-57-C5-7F-FB-6D-54-61-FD-D8-54-E5-9F-4C-E1-B8-DB-29-C2-EA-27-8A-DA-54-8B-1A-21-E7-A4-19-D4-3B-36-E2-17-21-C5" }
        };

        public User Create(User user)
        {
            _users.Add(user);

            return user;
        }

        public void Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public User Get(int userId)
            => _users.Find(u => u.Id == userId);

        public IEnumerable<User> GetAll()
            => _users;

        public void Update(User user, string password = null)
        {
            throw new System.NotImplementedException();
        }
    }

    public class UserJsonRepository : IUserRepository
    {
        private readonly IStorageProvider _storageProvider;

        public UserJsonRepository(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public User Create(User user)
        {
            return user;
        }

        public void Delete(int id)
        {
            throw new System.NotImplementedException();
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

        public void Update(User user, string password = null)
        {
            throw new System.NotImplementedException();
        }
    }
}