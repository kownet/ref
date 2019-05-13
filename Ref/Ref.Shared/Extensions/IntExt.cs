namespace Ref.Shared.Extensions
{
    public static class IntExt
    {
        public static int? ToNull(this int? value)
            => value.HasValue && value.Value != 0 ? value.Value : (int?)null;
    }
}