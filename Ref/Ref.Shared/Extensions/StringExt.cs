using System.Globalization;
using System.Linq;
using System.Text;

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

        public static string RemoveDiacritics(this string str)
        {
            if (null == str) return null;
            var chars =
                from c in str.Normalize(NormalizationForm.FormD).ToCharArray()
                let uc = CharUnicodeInfo.GetUnicodeCategory(c)
                where uc != UnicodeCategory.NonSpacingMark
                select c;

            var cleanStr = new string(chars.ToArray()).Normalize(NormalizationForm.FormC);

            return cleanStr;
        }
    }
}