using System.ComponentModel;

namespace Ref.Data.Models
{
    public enum MarketType
    {
        [Description("Rynek pierwotny")]
        Primary = 0,

        [Description("Rynek wtórny")]
        Secondary = 1
    }
}