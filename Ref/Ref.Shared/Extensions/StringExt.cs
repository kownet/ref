namespace Ref.Shared.Extensions
{
    public static class StringExt
    {
        public static string TextAfter(this string value, string search)
            => value.Substring(value.IndexOf(search) + search.Length);

        public static string RemoveLastIf(this string value, string toDelete)
        {
            if(!string.IsNullOrWhiteSpace(value))
            {
                return value.Substring(value.Length - 1, 1) == toDelete
                ? value.Remove(value.Length - 1).Trim()
                : string.Empty;
            }
            return string.Empty;            
        }
    }
}