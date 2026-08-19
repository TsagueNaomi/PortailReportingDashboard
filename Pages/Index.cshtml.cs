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
        public bool IsExplore { get; set; }
        public bool IsError { get; set; }
        public List<string> Breadcrumbs { get; set; } = new();
        public List<MenuItem> FeaturedReports { get; set; } = new();

        public void OnGet([FromQuery] string? report)
        {
            ReportId = report;

            if (string.IsNullOrWhiteSpace(ReportId) || ReportId.Equals("home", StringComparison.OrdinalIgnoreCase))
            {
                IsHome = true;
                CurrentReport = null;
                ViewData["Title"] = "Accueil";
                ViewData["IsHome"] = true;
                Breadcrumbs = new List<string> { "Portail", "Accueil" };
                
                // Load featured reports for homepage
                LoadFeaturedReports();
            }
            else
            {
                IsHome = false;
                CurrentReport = _navService.GetItemById(ReportId);
                
                if (CurrentReport == null)
                {
                    CurrentReport = new MenuItem
                    {
                        Id = ReportId,
                        Title = "Rapport introuvable",
                        Type = ItemType.Report,
                        Engine = ReportEngine.PowerBI,
                        Description = "Le rapport demandé n'existe pas ou a été déplacé."
                    };
                    IsError = true;
                }
                else
                {
                    // Point 3: Error state is triggered directly if the selected report has SimulateError enabled
                    IsError = CurrentReport.SimulateError;
                }

                ViewData["Title"] = CurrentReport.Title;
                ViewData["ActiveReportId"] = ReportId;
                ViewData["IsError"] = IsError;

                Breadcrumbs = _navService.GetBreadcrumbPath(ReportId);
                if (!Breadcrumbs.Any())
                {
                    Breadcrumbs = new List<string> { "Commercial", CurrentReport.Title };
                }
            }
        }

        private void LoadFeaturedReports()
        {
            // Get all active reports from navigation service
            var allItems = _navService.GetAllFlatItems();
            var allReports = allItems
                .Where(i => i.Type == ItemType.Report && i.IsActive)
                .ToList();

            // Randomly select between 8 and 12 reports (or all if total < 8)
            Random random = new Random();
            int targetCount = Math.Min(allReports.Count, Math.Max(8, random.Next(8, 13)));
            
            FeaturedReports = allReports
                .OrderBy(x => random.Next())
                .Take(targetCount)
                .OrderBy(r => r.Title)
                .ToList();
        }
    }
}
