using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortailSocadel.Models;
using PortailSocadel.Services;

namespace PortailSocadel.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly INavigationService _navService;

        public IndexModel(INavigationService navService)
        {
            _navService = navService;
        }

        [BindProperty(SupportsGet = true)]
        public string View { get; set; } = "datatable";

        public bool IsStatsView => string.Equals(View, "stats", StringComparison.OrdinalIgnoreCase);

        public List<MenuItem> TreeItems { get; set; } = new();
        public List<MenuItem> FlatItems { get; set; } = new();
        public List<MenuItem> CategoryOptions { get; set; } = new();
        public List<MenuItem> SubCategoryOptions { get; set; } = new();

        public int TotalCategories { get; set; }
        public int TotalSubCategories { get; set; }
        public int TotalReports { get; set; }
        public int TotalPowerBI { get; set; }
        public int TotalSSRS { get; set; }
        public int TotalExcel { get; set; }

        [BindProperty]
        public MenuItem ItemInput { get; set; } = new();

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            LoadData();
        }

        private void LoadData()
        {
            TreeItems = _navService.GetMenuTree();
            FlatItems = _navService.GetAllFlatItems();

            TotalCategories = FlatItems.Count(i => i.Type == ItemType.Category);
            TotalSubCategories = FlatItems.Count(i => i.Type == ItemType.SubCategory);
            TotalReports = FlatItems.Count(i => i.Type == ItemType.Report);

            TotalPowerBI = FlatItems.Count(i => i.Type == ItemType.Report && i.Engine == ReportEngine.PowerBI);
            TotalSSRS = FlatItems.Count(i => i.Type == ItemType.Report && i.Engine == ReportEngine.SSRS);
            TotalExcel = FlatItems.Count(i => i.Type == ItemType.Report && i.Engine == ReportEngine.Excel);

            CategoryOptions = FlatItems.Where(i => i.Type == ItemType.Category).OrderBy(i => i.Order).ToList();
            SubCategoryOptions = FlatItems.Where(i => i.Type == ItemType.SubCategory).OrderBy(i => i.Order).ToList();
        }

        public IActionResult OnPostAdd()
        {
            if (string.IsNullOrWhiteSpace(ItemInput.Title))
            {
                ErrorMessage = "Le titre de l'élément est obligatoire.";
                return RedirectToPage();
            }

            // If it's a category, ParentId must be null
            if (ItemInput.Type == ItemType.Category)
            {
                ItemInput.ParentId = null;
            }

            _navService.AddItem(ItemInput);
            SuccessMessage = $"L'élément « {ItemInput.Title} » a été ajouté avec succès dans l'arborescence.";
            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            if (string.IsNullOrWhiteSpace(ItemInput.Id) || string.IsNullOrWhiteSpace(ItemInput.Title))
            {
                ErrorMessage = "Informations manquantes pour la modification.";
                return RedirectToPage();
            }

            if (ItemInput.Type == ItemType.Category)
            {
                ItemInput.ParentId = null;
            }

            bool updated = _navService.UpdateItem(ItemInput);
            if (updated)
            {
                SuccessMessage = $"L'élément « {ItemInput.Title} » a été mis à jour avec succès.";
            }
            else
            {
                ErrorMessage = "Élément introuvable pour la mise à jour.";
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete([FromForm] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ErrorMessage = "Identifiant manquant pour la suppression.";
                return RedirectToPage();
            }

            var item = _navService.GetItemById(id);
            string title = item?.Title ?? id;

            bool deleted = _navService.DeleteItem(id);
            if (deleted)
            {
                SuccessMessage = $"L'élément « {title} » et ses éventuels sous-éléments ont été supprimés avec succès.";
            }
            else
            {
                ErrorMessage = "Impossible de supprimer cet élément.";
            }

            return RedirectToPage();
        }

        public IActionResult OnPostReset()
        {
            _navService.ResetToDefault();
            SuccessMessage = "L'arborescence complète a été réinitialisée avec les paramètres d'usine de SOCADEL.";
            return RedirectToPage();
        }
    }
}
