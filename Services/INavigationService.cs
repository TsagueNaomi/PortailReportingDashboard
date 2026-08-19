using PortailSocadel.Models;

namespace PortailSocadel.Services
{
    public interface INavigationService
    {
        List<MenuItem> GetMenuTree();
        List<MenuItem> GetAllFlatItems();
        MenuItem? GetItemById(string id);
        MenuItem? GetItemByTitleAndParents(string title, string? subCategoryTitle = null, string? categoryTitle = null);
        void AddItem(MenuItem item);
        bool UpdateItem(MenuItem item);
        bool DeleteItem(string id);
        List<MenuItem> SearchItems(string query);
        List<string> GetBreadcrumbPath(string itemId);
        List<MenuItem> GetBreadcrumbNodes(string itemId);
        void ResetToDefault();
    }
}
