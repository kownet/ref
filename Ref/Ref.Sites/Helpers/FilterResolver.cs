using Ref.Data.Models;

namespace Ref.Sites.Helpers
{
    public static class FilterResolver
    {
        public static string Type(SiteType siteType, Filter filter)
        {
            switch (siteType)
            {
                case SiteType.OtoDom:
                    {
                        switch (filter.Type)
                        {
                            case PropertyType.Flat: return "mieszkanie";
                            case PropertyType.House: return "dom";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Olx:
                    {
                        switch (filter.Type)
                        {
                            case PropertyType.Flat: return "mieszkania";
                            case PropertyType.House: return "domy";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Adresowo:
                    {
                        switch (filter.Type)
                        {
                            case PropertyType.Flat: return "mieszkania";
                            case PropertyType.House: return "domy";
                            default: return string.Empty;
                        };
                    }
                case SiteType.DomiPorta:
                    {
                        switch (filter.Type)
                        {
                            case PropertyType.Flat: return "mieszkanie";
                            case PropertyType.House: return "dom";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gratka:
                    {
                        switch (filter.Type)
                        {
                            case PropertyType.Flat: return "mieszkania";
                            case PropertyType.House: return "domy";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Morizon:
                    {
                        switch (filter.Type)
                        {
                            case PropertyType.Flat: return "mieszkania";
                            case PropertyType.House: return "domy";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gumtree:
                    {
                        switch (filter.Type)
                        {
                            case PropertyType.Flat: return "s-mieszkania-i-domy";
                            case PropertyType.House: return "s-mieszkania-i-domy";
                            default: return string.Empty;
                        };
                    }
                default: return string.Empty;
            }
        }

        public static string Deal(SiteType siteType, Filter filter)
        {
            switch (siteType)
            {
                case SiteType.OtoDom:
                    {
                        switch (filter.Deal)
                        {
                            case DealType.Sale: return "sprzedaz";
                            case DealType.Rent: return "wynajem";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Olx:
                    {
                        switch (filter.Deal)
                        {
                            case DealType.Sale: return "sprzedaz";
                            case DealType.Rent: return "wynajem";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Adresowo:
                    {
                        switch (filter.Deal)
                        {
                            case DealType.Sale: return string.Empty;
                            case DealType.Rent: return string.Empty;
                            default: return string.Empty;
                        };
                    }
                case SiteType.DomiPorta:
                    {
                        switch (filter.Deal)
                        {
                            case DealType.Sale: return "sprzedam";
                            case DealType.Rent: return "wynajme";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gratka:
                    {
                        switch (filter.Deal)
                        {
                            case DealType.Sale: return "sprzedaz";
                            case DealType.Rent: return "wynajem";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Morizon:
                    {
                        switch (filter.Deal)
                        {
                            case DealType.Sale: return string.Empty;
                            case DealType.Rent: return "do-wynajecia";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gumtree:
                    {
                        switch (filter.Deal)
                        {
                            case DealType.Sale: return "-sprzedam-i-kupie";
                            case DealType.Rent: return "-do-wynajecia";
                            default: return string.Empty;
                        };
                    }
                default: return string.Empty;
            }
        }

        public static string Market(SiteType siteType, Filter filter)
        {
            switch (siteType)
            {
                case SiteType.OtoDom:
                    {
                        switch (filter.Market)
                        {
                            case MarketType.Primary: return "";
                            case MarketType.Secondary: return "secondary";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Olx:
                    {
                        switch (filter.Market)
                        {
                            case MarketType.Primary: return "primary";
                            case MarketType.Secondary: return "secondary";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Adresowo:
                    {
                        switch (filter.Market)
                        {
                            case MarketType.Primary: return string.Empty;
                            case MarketType.Secondary: return "fuz";
                            default: return string.Empty;
                        };
                    }
                case SiteType.DomiPorta:
                    {
                        switch (filter.Market)
                        {
                            case MarketType.Primary: return string.Empty;
                            case MarketType.Secondary: return "Wtorny";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gratka:
                    {
                        switch (filter.Market)
                        {
                            case MarketType.Primary: return "pierwotny";
                            case MarketType.Secondary: return "wtorny";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Morizon:
                    {
                        switch (filter.Market)
                        {
                            case MarketType.Primary: return "1";
                            case MarketType.Secondary: return "2";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gumtree:
                    {
                        switch (filter.Market)
                        {
                            case MarketType.Primary: return string.Empty;
                            case MarketType.Secondary: return string.Empty;
                            default: return string.Empty;
                        };
                    }
                default: return string.Empty;
            }
        }

        public static string Code(Filter filter)
        {
            /// TODO: cities!
            switch (filter.Location)
            {
                case "warszawa":
                    return filter.Deal == DealType.Sale ? "v1c9073l3200008a1dwp" : "v1c9008l3200008a1dwp";
                case "kraków":
                    return filter.Deal == DealType.Sale ? "v1c9073l3200208a1dwp" : "v1c9008l3200208p";
                case "legnica":
                    return "v1c9073l3200096a1dwp";
                case "opole":
                    return filter.Deal == DealType.Sale ? "v1c9073l3200234a1dwp" : "v1c9008l3200234p";
                case "poznań":
                    return filter.Deal == DealType.Sale ? "v1c9073l3200366a1dwp" : "v1c9008l3200366a1dwp";
                case "puławy":
                    return "v1c9073l3200150a1dwp";
                case "płock":
                    return "v1c9073l3200071a1dwp";
                case "rzeszów":
                    return "v1c9073l3200252a1dwp";
                case "wrocław":
                    return "v1c9073l3200114a1dwp";
                case "łódź":
                    return "v1c9073l3200183a1dwp";
                default: return string.Empty;
            }
        }
    }
}