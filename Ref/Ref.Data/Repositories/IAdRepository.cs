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
        IEnumerable<Ad> GetAll();
        void SaveAll(IEnumerable<Ad> ads);
    }

    public class JsonRepository : IAdRepository
    {
        private readonly IStorageProvider _storageProvider;

        public JsonRepository(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public IEnumerable<Ad> GetAll()
        {
            var result = new List<Ad>();

            var currentContent = File.ReadAllText(_storageProvider.FullPath());

            var currentContentList = JsonConvert.DeserializeObject<List<Ad>>(currentContent);

            if (currentContentList.AnyAndNotNull())
            {
                result.AddRange(currentContentList);
            }

            return result;
        }

        public void SaveAll(IEnumerable<Ad> ads)
        {
            var json = JsonConvert.SerializeObject(ads);

            if (!string.IsNullOrWhiteSpace(json))
            {
                File.WriteAllText(
                    _storageProvider.FullPath(),
                    json
                );
            }
        }
    }
}