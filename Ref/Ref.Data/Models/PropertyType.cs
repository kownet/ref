using System.ComponentModel;

namespace Ref.Data.Models
{
    public enum PropertyType
    {
        [Description("Mieszkanie")]
        Flat = 0,

        [Description("Dom")]
        House = 1,

        [Description("Działka")]
        Plot = 2
    }
}