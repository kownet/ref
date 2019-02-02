using System.IO;

namespace Ref.Shared.Providers
{
    public interface IStorageProvider
    {
        string ResultPath();
        string ResultFullPath(string clientCode);

        string ClientsPath();
        string ClientsFullPath(string clientCode);
    }

    public class StorageProvider : IStorageProvider
    {
        private readonly string _resultPath;
        private readonly string _clientsPath;

        public StorageProvider(string resultPath, string clientsPath)
        {
            _resultPath = resultPath;
            _clientsPath = clientsPath;
        }

        public string ResultPath() => _resultPath;
        public string ClientsPath() => _clientsPath;

        public string ResultFullPath(string clientCode) => Path.Combine(ResultPath(), $"{clientCode}-result.json");
        public string ClientsFullPath(string clientCode) => Path.Combine(ClientsPath(), $"{clientCode}.json");
    }
}