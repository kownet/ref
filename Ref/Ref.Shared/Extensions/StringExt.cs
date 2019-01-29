namespace Ref.Shared.Extensions
{
    public static class StringExt
    {
        public static string TextAfter(this string value, string search)
            => value.Substring(value.IndexOf(search) + search.Length);
    }
}