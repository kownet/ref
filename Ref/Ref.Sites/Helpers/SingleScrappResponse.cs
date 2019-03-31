namespace Ref.Sites.Helpers
{
    public class SingleScrappResponse
    {
        public int Floor { get; set; }
        public string Content { get; set; }
        public bool Succeed => !string.IsNullOrWhiteSpace(Content);
    }
}