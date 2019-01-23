using Ref.Shared.Utils;
using System.IO;

namespace Ref.Shared.Providers
{
    public interface IStorageProvider
    {
        string Dir();
        string Client();
        string FullPath();
    }

    public class StorageProvider : IStorageProvider
    {
        private readonly string _dir;
        private readonly string _client;

        public StorageProvider(string dir, string client)
        {
            _dir = dir;
            _client = client;
        }

        public string Dir() => _dir;
        public string Client() => _client;

        public string FullPath() => Path.Combine(_dir, StorageFile.Name(Client()));
    }
}