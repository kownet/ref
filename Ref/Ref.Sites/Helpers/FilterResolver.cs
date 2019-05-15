using Ref.Data.Models;

namespace Ref.Sites.Helpers
{
    public static class FilterResolver
    {
        public static string Type(SiteType siteType, PropertyType propertyType)
        {
            switch (siteType)
            {
                case SiteType.OtoDom:
                    {
                        switch (propertyType)
                        {
                            case PropertyType.Flat: return "mieszkanie";
                            case PropertyType.House: return "dom";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Olx:
                    {
                        switch (propertyType)
                        {
                            case PropertyType.Flat: return "mieszkania";
                            case PropertyType.House: return "domy";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Adresowo:
                    {
                        switch (propertyType)
                        {
                            case PropertyType.Flat: return "mieszkania";
                            case PropertyType.House: return "domy";
                            default: return string.Empty;
                        };
                    }
                case SiteType.DomiPorta:
                    {
                        switch (propertyType)
                        {
                            case PropertyType.Flat: return "mieszkanie";
                            case PropertyType.House: return "dom";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gratka:
                    {
                        switch (propertyType)
                        {
                            case PropertyType.Flat: return "mieszkania";
                            case PropertyType.House: return "domy";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Morizon:
                    {
                        switch (propertyType)
                        {
                            case PropertyType.Flat: return "mieszkania";
                            case PropertyType.House: return "domy";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gumtree:
                    {
                        switch (propertyType)
                        {
                            case PropertyType.Flat: return "s-mieszkania-i-domy";
                            case PropertyType.House: return "s-mieszkania-i-domy";
                            default: return string.Empty;
                        };
                    }
                default: return string.Empty;
            }
        }

        public static string Deal(SiteType siteType, DealType dealType)
        {
            switch (siteType)
            {
                case SiteType.OtoDom:
                    {
                        switch (dealType)
                        {
                            case DealType.Sale: return "sprzedaz";
                            case DealType.Rent: return "wynajem";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Olx:
                    {
                        switch (dealType)
                        {
                            case DealType.Sale: return "sprzedaz";
                            case DealType.Rent: return "wynajem";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Adresowo:
                    {
                        switch (dealType)
                        {
                            case DealType.Sale: return string.Empty;
                            case DealType.Rent: return string.Empty;
                            default: return string.Empty;
                        };
                    }
                case SiteType.DomiPorta:
                    {
                        switch (dealType)
                        {
                            case DealType.Sale: return "sprzedam";
                            case DealType.Rent: return "wynajme";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gratka:
                    {
                        switch (dealType)
                        {
                            case DealType.Sale: return "sprzedaz";
                            case DealType.Rent: return "wynajem";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Morizon:
                    {
                        switch (dealType)
                        {
                            case DealType.Sale: return string.Empty;
                            case DealType.Rent: return "do-wynajecia";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gumtree:
                    {
                        switch (dealType)
                        {
                            case DealType.Sale: return "-sprzedam-i-kupie";
                            case DealType.Rent: return "-do-wynajecia";
                            default: return string.Empty;
                        };
                    }
                default: return string.Empty;
            }
        }

        public static string Market(SiteType siteType, MarketType marketType)
        {
            switch (siteType)
            {
                case SiteType.OtoDom:
                    {
                        switch (marketType)
                        {
                            case MarketType.Primary: return "";
                            case MarketType.Secondary: return "secondary";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Olx:
                    {
                        switch (marketType)
                        {
                            case MarketType.Primary: return "primary";
                            case MarketType.Secondary: return "secondary";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Adresowo:
                    {
                        switch (marketType)
                        {
                            case MarketType.Primary: return string.Empty;
                            case MarketType.Secondary: return "fuz_l";
                            default: return string.Empty;
                        };
                    }
                case SiteType.DomiPorta:
                    {
                        switch (marketType)
                        {
                            case MarketType.Primary: return string.Empty;
                            case MarketType.Secondary: return "Wtorny";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gratka:
                    {
                        switch (marketType)
                        {
                            case MarketType.Primary: return "pierwotny";
                            case MarketType.Secondary: return "wtorny";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Morizon:
                    {
                        switch (marketType)
                        {
                            case MarketType.Primary: return "1";
                            case MarketType.Secondary: return "2";
                            default: return string.Empty;
                        };
                    }
                case SiteType.Gumtree:
                    {
                        switch (marketType)
                        {
                            case MarketType.Primary: return string.Empty;
                            case MarketType.Secondary: return string.Empty;
                            default: return string.Empty;
                        };
                    }
                default: return string.Empty;
            }
        }
    }
}