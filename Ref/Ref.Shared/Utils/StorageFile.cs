namespace Ref.Shared.Utils
{
    public static class StorageFile
    {
        public static string Name(string clientId) => $"{clientId}-storage.json";
    }
}