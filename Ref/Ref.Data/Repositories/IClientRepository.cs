using Newtonsoft.Json;
using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using System.Collections.Generic;
using System.IO;

namespace Ref.Data.Repositories
{
    public interface IClientRepository : IRepository
    {
        IEnumerable<Client> GetAll();
    }

    public class ClientJsonRepository : IClientRepository
    {
        private readonly IStorageProvider _storageProvider;

        public ClientJsonRepository(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public IEnumerable<Client> GetAll()
        {
            var result = new List<Client>();

            var files = Directory.GetFiles(_storageProvider.ClientsPath(), "*.json", SearchOption.TopDirectoryOnly);

            if (files.AnyAndNotNull())
            {
                foreach (var file in files)
                {
                    var clientRaw = File.ReadAllText(file);

                    if (!string.IsNullOrWhiteSpace(clientRaw))
                    {
                        var clientEntity = JsonConvert.DeserializeObject<Client>(clientRaw);

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