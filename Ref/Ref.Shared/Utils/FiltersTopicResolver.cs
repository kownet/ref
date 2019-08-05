using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ref.Shared.Utils
{
    public static class FiltersTopicResolver
    {
        private static readonly int _maxLenght = 20;

        public static string GetAbbreviation(IEnumerable<string> filterNames)
        {
            if (filterNames.Count() == 1)
                return filterNames.First();

            if (filterNames.Count() > 1)
            {
                var sb = new StringBuilder();

                foreach (var title in filterNames)
                {
                    sb.Append($"{title.PadRight(_maxLenght).Substring(0, _maxLenght).TrimEnd()}...");
                }

                return sb.ToString();
            }
            else
                return string.Empty;
        }

        public static string GetAbbreviation(string filterName)
        {
            if (string.IsNullOrWhiteSpace(filterName))
                return string.Empty;
            else
            {
                return $"{filterName.PadRight(_maxLenght).Substring(0, _maxLenght).TrimEnd()}...";
            }
        }
    }
}