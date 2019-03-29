namespace Ref.Data.Models
{
    public class Site
    {
        public int Id { get; set; }
        public SiteType Type { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}