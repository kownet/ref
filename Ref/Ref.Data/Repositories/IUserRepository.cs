using Newtonsoft.Json;
using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using System.Collections.Generic;
using System.IO;

namespace Ref.Data.Repositories
{
    public interface IUserRepository : IRepository
    {
        IEnumerable<User> GetAll();
    }

    public class UserJsonRepository : IUserRepository
    {
        private readonly IStorageProvider _storageProvider;

        public UserJsonRepository(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

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
    }
}