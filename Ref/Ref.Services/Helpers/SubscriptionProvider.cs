using Ref.Data.Models;

namespace Ref.Services.Helpers
{
    public static class SubscriptionProvider
    {
        public static int MaxNumberOfFilters(SubscriptionType subscriptionType)
        {
            switch (subscriptionType)
            {
                case SubscriptionType.Normal:
                    return 4;
                case SubscriptionType.Premium:
                    return 19;
                case SubscriptionType.Special:
                    return 99;
                case SubscriptionType.Undefinded:
                    return 0;
                default: return 0;
            }
        }
    }
}