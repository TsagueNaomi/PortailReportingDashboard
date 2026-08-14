namespace PortailSocadel.Models
{
    public enum ItemType
    {
        Category = 1,     // Level 1: e.g. Commercial, Finance
        SubCategory = 2,  // Level 2: e.g. Recouvrement, Facturation
        Report = 3        // Level 3: e.g. Rapport A, Rapport B
    }

    public enum ReportEngine
    {
        PowerBI,
        SSRS,
        Excel,
        Custom
    }

    public class MenuItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
        public string Title { get; set; } = string.Empty;
        public string? ParentId { get; set; }
        public ItemType Type { get; set; } = ItemType.Report;
        public int Level => (int)Type;
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public ReportEngine Engine { get; set; } = ReportEngine.PowerBI;
        public string? Description { get; set; }
        public string? Code { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties for tree hierarchy
        public List<MenuItem> Children { get; set; } = new();
    }
}
