using Microsoft.EntityFrameworkCore;
using PortailSocadel.Data;
using PortailSocadel.Models;

namespace PortailSocadel.Services
{
    public class DbNavigationService : INavigationService
    {
        private readonly AppDbContext _context;

        public DbNavigationService(AppDbContext context)
        {
            _context = context;
        }

        public List<MenuItem> GetMenuTree()
        {
            var allItems = _context.MenuItems.ToList();
            var roots = allItems
                .Where(i => string.IsNullOrEmpty(i.ParentId))
                .OrderBy(i => i.Order)
                .Select(CloneItem)
                .ToList();

            foreach (var root in roots)
            {
                PopulateChildren(root, allItems);
            }

            return roots;
        }

        private void PopulateChildren(MenuItem parent, List<MenuItem> allItems)
        {
            var children = allItems
                .Where(i => i.ParentId == parent.Id)
                .OrderBy(i => i.Order)
                .Select(CloneItem)
                .ToList();

            parent.Children = children;

            foreach (var child in children)
            {
                PopulateChildren(child, allItems);
            }
        }

        private static MenuItem CloneItem(MenuItem source)
        {
            return new MenuItem
            {
                Id = source.Id,
                Title = source.Title,
                ParentId = source.ParentId,
                Type = source.Type,
                Order = source.Order,
                IsActive = source.IsActive,
                Engine = source.Engine,
                Description = source.Description,
                ReportUrl = source.ReportUrl,
                Code = source.Code,
                SimulateError = source.SimulateError,
                CreatedAt = source.CreatedAt,
                UpdatedAt = source.UpdatedAt,
                Children = new List<MenuItem>()
            };
        }

        public List<MenuItem> GetAllFlatItems()
        {
            return _context.MenuItems.OrderBy(i => i.Type).ThenBy(i => i.Order).ToList();
        }

        public MenuItem? GetItemById(string id)
        {
            var item = _context.MenuItems.FirstOrDefault(i => i.Id == id);
            return item != null ? CloneItem(item) : null;
        }

        public MenuItem? GetItemByTitleAndParents(string title, string? subCategoryTitle = null, string? categoryTitle = null)
        {
            var allItems = _context.MenuItems.ToList();
            var report = allItems.FirstOrDefault(i => i.Type == ItemType.Report && i.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            
            if (report != null && !string.IsNullOrEmpty(subCategoryTitle))
            {
                var parent = allItems.FirstOrDefault(i => i.Id == report.ParentId);
                if (parent != null && parent.Title.Equals(subCategoryTitle, StringComparison.OrdinalIgnoreCase))
                {
                    return CloneItem(report);
                }
                return null; // Parent title mismatch
            }
            return report != null ? CloneItem(report) : null;
        }

        public void AddItem(MenuItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Guid.NewGuid().ToString("N")[..8];
            }

            if (item.Type == ItemType.Category)
            {
                var maxCatOrder = _context.MenuItems.Where(i => i.Type == ItemType.Category).Select(i => (int?)i.Order).Max() ?? 0;
                item.Order = maxCatOrder + 1;
            }
            else if (item.Type == ItemType.SubCategory)
            {
                var maxSubOrder = _context.MenuItems.Where(i => i.Type == ItemType.SubCategory && i.ParentId == item.ParentId).Select(i => (int?)i.Order).Max() ?? 0;
                item.Order = maxSubOrder + 1;
            }
            else if (item.Type == ItemType.Report)
            {
                var maxRepOrder = _context.MenuItems.Where(i => i.Type == ItemType.Report && i.ParentId == item.ParentId).Select(i => (int?)i.Order).Max() ?? 0;
                item.Order = maxRepOrder + 1;
            }

            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
            
            _context.MenuItems.Add(item);
            _context.SaveChanges();
            DbSeeder.SaveData(_context);
        }

        public bool UpdateItem(MenuItem item)
        {
            var existing = _context.MenuItems.FirstOrDefault(i => i.Id == item.Id);
            if (existing == null) return false;

            existing.Title = item.Title;
            existing.ParentId = item.ParentId;
            existing.Type = item.Type;
            existing.Order = item.Order;
            existing.IsActive = item.IsActive;
            existing.Engine = item.Engine;
            existing.Description = item.Description;
            existing.ReportUrl = item.ReportUrl;
            existing.Code = item.Code;
            existing.SimulateError = item.SimulateError;
            existing.UpdatedAt = DateTime.UtcNow;
            
            _context.SaveChanges();
            DbSeeder.SaveData(_context);
            return true;
        }

        public bool DeleteItem(string id)
        {
            var item = _context.MenuItems.FirstOrDefault(i => i.Id == id);
            if (item == null) return false;

            var toDelete = new List<string> { id };
            FindDescendantIds(id, toDelete);

            var itemsToDelete = _context.MenuItems.Where(i => toDelete.Contains(i.Id)).ToList();
            _context.MenuItems.RemoveRange(itemsToDelete);
            _context.SaveChanges();
            DbSeeder.SaveData(_context);
            return true;
        }

        private void FindDescendantIds(string parentId, List<string> list)
        {
            var children = _context.MenuItems.Where(i => i.ParentId == parentId).Select(i => i.Id).ToList();
            foreach (var childId in children)
            {
                list.Add(childId);
                FindDescendantIds(childId, list);
            }
        }

        public List<MenuItem> SearchItems(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<MenuItem>();

            return _context.MenuItems
                .Where(i => i.Title.Contains(query) ||
                            (!string.IsNullOrEmpty(i.Description) && i.Description.Contains(query)) ||
                            (!string.IsNullOrEmpty(i.Code) && i.Code.Contains(query)))
                .ToList()
                .Select(CloneItem)
                .ToList();
        }

        public List<string> GetBreadcrumbPath(string itemId)
        {
            return GetBreadcrumbNodes(itemId).Select(i => i.Title).ToList();
        }

        public List<MenuItem> GetBreadcrumbNodes(string itemId)
        {
            var list = new List<MenuItem>();
            var allItems = _context.MenuItems.ToList();
            var current = allItems.FirstOrDefault(i => i.Id == itemId);
            
            while (current != null)
            {
                list.Insert(0, CloneItem(current));
                current = !string.IsNullOrEmpty(current.ParentId)
                    ? allItems.FirstOrDefault(i => i.Id == current.ParentId)
                    : null;
            }
            return list;
        }

        public void ResetToDefault()
        {
            var dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "portailsocadel_store.json");
            if (File.Exists(dataFilePath))
            {
                try
                {
                    File.Delete(dataFilePath);
                }
                catch { }
            }
            DbSeeder.ReSeedDefaults(_context);
        }
    }
}
