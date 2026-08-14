using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortailSocadel.Models;
using PortailSocadel.Services;

namespace PortailSocadel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly INavigationService _navService;

        public IndexModel(INavigationService navService)
        {
            _navService = navService;
        }

        public string? ReportId { get; set; }
        public MenuItem? CurrentReport { get; set; }
        public bool IsHome { get; set; }
        public bool IsError { get; set; }
        public List<string> Breadcrumbs { get; set; } = new();

        public void OnGet([FromQuery] string? report, [FromQuery] string? error)
        {
            ReportId = report;
            IsError = !string.IsNullOrEmpty(error) && (error == "1" || error.Equals("true", StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(ReportId) || ReportId.Equals("home", StringComparison.OrdinalIgnoreCase))
            {
                IsHome = true;
                ViewData["Title"] = "Accueil";
                ViewData["IsHome"] = true;
                Breadcrumbs = new List<string> { "Commercial" };
            }
            else
            {
                IsHome = false;
                CurrentReport = _navService.GetItemById(ReportId);
                
                // Fallback if not found directly
                if (CurrentReport == null)
                {
                    CurrentReport = new MenuItem
                    {
                        Id = ReportId,
                        Title = "Rapport A",
                        Type = ItemType.Report,
                        Engine = ReportEngine.PowerBI
                    };
                }

                ViewData["Title"] = CurrentReport.Title;
                ViewData["ActiveReportId"] = ReportId;
                ViewData["IsError"] = IsError;

                Breadcrumbs = _navService.GetBreadcrumbPath(ReportId);
                if (!Breadcrumbs.Any())
                {
                    Breadcrumbs = new List<string> { "Commercial", "Recouvrement", CurrentReport.Title };
                }
            }
        }
    }
}
