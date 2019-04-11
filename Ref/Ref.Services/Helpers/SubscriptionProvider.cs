using Ref.Data.Models;

namespace Ref.Services.Helpers
{
    public static class SubscriptionProvider
    {
        public static int MaxNumberOfFilters(SubscriptionType subscriptionType)
        {
            switch (subscriptionType)
            {
                case SubscriptionType.Demo:
                    return 0;
                case SubscriptionType.Normal:
                    return 2;
                case SubscriptionType.Premium:
                    return 9;
                case SubscriptionType.Special:
                    return 99;
                case SubscriptionType.Undefinded:
                    return 0;
                default: return 0;
            }
        }
    }
}