using Newtonsoft.Json;
using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using System.Collections.Generic;
using System.IO;

namespace Ref.Data.Repositories
{
    public interface IAdRepository : IRepository
    {
        IEnumerable<Ad> GetAll(string clientCode);
        void SaveAll(string clientCode, IEnumerable<Ad> ads);
    }

    public class AdJsonRepository : IAdRepository
    {
        private readonly IStorageProvider _storageProvider;

        public AdJsonRepository(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public IEnumerable<Ad> GetAll(string clientCode)
        {
            var result = new List<Ad>();

            var clientPath = _storageProvider.ResultFullPath(clientCode);

            if(File.Exists(clientPath))
            {
                var currentContent = File.ReadAllText(clientPath);

                var currentContentList = JsonConvert.DeserializeObject<List<Ad>>(currentContent);

                if (currentContentList.AnyAndNotNull())
                {
                    result.AddRange(currentContentList);
                }
            }

            return result;
        }

        public void SaveAll(string clientCode, IEnumerable<Ad> ads)
        {
            var json = JsonConvert.SerializeObject(ads);

            if (!string.IsNullOrWhiteSpace(json))
            {
                File.WriteAllText(
                    _storageProvider.ResultFullPath(clientCode),
                    json
                );
            }
        }
    }
}