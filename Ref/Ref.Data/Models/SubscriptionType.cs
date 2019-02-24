using System.ComponentModel;

namespace Ref.Data.Models
{
    public enum SubscriptionType
    {
        [Description("Normalny")]
        Normal = 0,

        [Description("Premium")]
        Premium = 1,

        [Description("Specjalny")]
        Special = 2,

        [Description("Niezdefiniowany")]
        Undefinded = 100,
    }
}