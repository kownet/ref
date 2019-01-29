using Ref.Data.Models;
using Ref.Shared.Providers;
using Ref.Shared.Utils;

namespace Ref.Sites.Helpers
{
    public static class FilterResolver
    {
        public static string Type(SiteType siteType, IFilterProvider filter)
        {
            var typeEnum = (PropertyTypes)filter.Type();

            switch (siteType)
            {
                case SiteType.OtoDom:
                    {
                        switch (typeEnum)
                        {
                            case PropertyTypes.Flat: return "mieszkanie";
                            case PropertyTypes.House: return "dom";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Olx:
                    {
                        switch (typeEnum)
                        {
                            case PropertyTypes.Flat: return "mieszkania";
                            case PropertyTypes.House: return "domy";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Adresowo:
                    {
                        switch (typeEnum)
                        {
                            case PropertyTypes.Flat: return "mieszkania";
                            case PropertyTypes.House: return "domy";
                            default: return string.Empty;
                        };
                    }
                case SiteType.DomiPorta:
                    {
                        switch (typeEnum)
                        {
                            case PropertyTypes.Flat: return "mieszkanie";
                            case PropertyTypes.House: return "dom";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gratka:
                    {
                        switch (typeEnum)
                        {
                            case PropertyTypes.Flat: return "mieszkania";
                            case PropertyTypes.House: return "domy";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Morizon:
                    {
                        switch (typeEnum)
                        {
                            case PropertyTypes.Flat: return "mieszkania";
                            case PropertyTypes.House: return "domy";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gumtree:
                    {
                        switch (typeEnum)
                        {
                            case PropertyTypes.Flat: return "s-mieszkania-i-domy";
                            case PropertyTypes.House: return "s-mieszkania-i-domy";
                            default: return string.Empty;
                        };
                    }
                default: return string.Empty;
            }
        }

        public static string Deal(SiteType siteType, IFilterProvider filter)
        {
            var dealEnum = (DealTypes)filter.Deal();

            switch (siteType)
            {
                case SiteType.OtoDom:
                    {
                        switch (dealEnum)
                        {
                            case DealTypes.Sale: return "sprzedaz";
                            case DealTypes.Rent: return "wynajem";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Olx:
                    {
                        switch (dealEnum)
                        {
                            case DealTypes.Sale: return "sprzedaz";
                            case DealTypes.Rent: return "wynajem";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Adresowo:
                    {
                        switch (dealEnum)
                        {
                            case DealTypes.Sale: return string.Empty;
                            case DealTypes.Rent: return string.Empty;
                            default: return string.Empty;
                        };
                    }
                case SiteType.DomiPorta:
                    {
                        switch (dealEnum)
                        {
                            case DealTypes.Sale: return "sprzedam";
                            case DealTypes.Rent: return "wynajme";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gratka:
                    {
                        switch (dealEnum)
                        {
                            case DealTypes.Sale: return "sprzedaz";
                            case DealTypes.Rent: return "wynajem";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Morizon:
                    {
                        switch (dealEnum)
                        {
                            case DealTypes.Sale: return string.Empty;
                            case DealTypes.Rent: return "do-wynajecia";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gumtree:
                    {
                        switch (dealEnum)
                        {
                            case DealTypes.Sale: return "-sprzedam-i-kupie";
                            case DealTypes.Rent: return "-do-wynajecia";
                            default: return string.Empty;
                        };
                    }
                default: return string.Empty;
            }
        }

        public static string Market(SiteType siteType, IFilterProvider filter)
        {
            var marketEnum = (MarketTypes)filter.Market();

            switch (siteType)
            {
                case SiteType.OtoDom:
                    {
                        switch (marketEnum)
                        {
                            case MarketTypes.Primary: return ""; // TODO: nowe-Type()
                            case MarketTypes.Secondary: return "secondary";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Olx:
                    {
                        switch (marketEnum)
                        {
                            case MarketTypes.Primary: return "primary";
                            case MarketTypes.Secondary: return "secondary";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Adresowo:
                    {
                        switch (marketEnum)
                        {
                            case MarketTypes.Primary: return string.Empty;
                            case MarketTypes.Secondary: return "fuz";
                            default: return string.Empty;
                        };
                    }
                case SiteType.DomiPorta:
                    {
                        switch (marketEnum)
                        {
                            case MarketTypes.Primary: return string.Empty;
                            case MarketTypes.Secondary: return "Wtorny";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gratka:
                    {
                        switch (marketEnum)
                        {
                            case MarketTypes.Primary: return "pierwotny";
                            case MarketTypes.Secondary: return "wtorny";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Morizon:
                    {
                        switch (marketEnum)
                        {
                            case MarketTypes.Primary: return "1";
                            case MarketTypes.Secondary: return "2";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gumtree:
                    {
                        switch (marketEnum)
                        {
                            case MarketTypes.Primary: return string.Empty;
                            case MarketTypes.Secondary: return string.Empty;
                            default: return string.Empty;
                        };
                    }
                default: return string.Empty;
            }
        }

        public static string Code(IFilterProvider filter)
        {
            /// TODO: cities!
            switch (filter.Location())
            {
                case "warszawa": return "v1c9073l3200008a1dwp";
                default: return string.Empty;
            }
        }
    }
}