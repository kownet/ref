namespace Ref.Services.Features.Shared
{
    public abstract class BaseResult
    {
        public bool Succeed => string.IsNullOrWhiteSpace(Message);
        public string Message { get; set; }
    }
}