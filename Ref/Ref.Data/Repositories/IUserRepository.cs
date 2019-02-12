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