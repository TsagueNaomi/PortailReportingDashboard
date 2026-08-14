using PortailSocadel.Models;

namespace PortailSocadel.Services
{
    public class InMemoryNavigationService : INavigationService
    {
        private readonly List<MenuItem> _items = new();
        private readonly object _lock = new();

        public InMemoryNavigationService()
        {
            InitializeDefaultData();
        }

        private void InitializeDefaultData()
        {
            lock (_lock)
            {
                _items.Clear();

                // 1. Commercial (Level 1)
                var commercial = new MenuItem
                {
                    Id = "menu-commercial",
                    Title = "Commercial",
                    Type = ItemType.Category,
                    Order = 1,
                    IsActive = true,
                    Description = "Direction Commerciale et Clientèle"
                };
                _items.Add(commercial);

                // 1.1 Encaissement
                var encaissement = new MenuItem
                {
                    Id = "sub-encaissement",
                    Title = "Encaissement",
                    ParentId = commercial.Id,
                    Type = ItemType.SubCategory,
                    Order = 1,
                    IsActive = true,
                    Description = "Suivi des paiements et des caisses"
                };
                _items.Add(encaissement);
                _items.Add(new MenuItem { Id = "rep-enc-a", Title = "Rapport A", ParentId = encaissement.Id, Type = ItemType.Report, Order = 1, Engine = ReportEngine.PowerBI, Description = "Synthèse des encaissements par agence" });
                _items.Add(new MenuItem { Id = "rep-enc-b", Title = "Rapport B", ParentId = encaissement.Id, Type = ItemType.Report, Order = 2, Engine = ReportEngine.PowerBI, Description = "Détail journalier des modes de paiement" });

                // 1.2 Facturation
                var facturation = new MenuItem
                {
                    Id = "sub-facturation",
                    Title = "Facturation",
                    ParentId = commercial.Id,
                    Type = ItemType.SubCategory,
                    Order = 2,
                    IsActive = true,
                    Description = "Gestion des émissions de factures"
                };
                _items.Add(facturation);
                _items.Add(new MenuItem { Id = "rep-fac-a", Title = "Rapport A", ParentId = facturation.Id, Type = ItemType.Report, Order = 1, Engine = ReportEngine.PowerBI, Description = "Tableau de bord facturation mensuelle" });
                _items.Add(new MenuItem { Id = "rep-fac-b", Title = "Rapport B", ParentId = facturation.Id, Type = ItemType.Report, Order = 2, Engine = ReportEngine.PowerBI, Description = "Analyse des volumes et tranches tarifaires" });

                // 1.3 Recouvrement
                var recouvrement = new MenuItem
                {
                    Id = "sub-recouvrement",
                    Title = "Recouvrement",
                    ParentId = commercial.Id,
                    Type = ItemType.SubCategory,
                    Order = 3,
                    IsActive = true,
                    Description = "Suivi des impayés et relances"
                };
                _items.Add(recouvrement);
                _items.Add(new MenuItem { Id = "rep-rec-a", Title = "Rapport A", ParentId = recouvrement.Id, Type = ItemType.Report, Order = 1, Engine = ReportEngine.PowerBI, Description = "Tableau de bord de recouvrement commercial" });
                _items.Add(new MenuItem { Id = "rep-rec-b", Title = "Rapport B", ParentId = recouvrement.Id, Type = ItemType.Report, Order = 2, Engine = ReportEngine.PowerBI, Description = "Balance âgée et actions contentieuses" });

                // 1.4 Abonnement
                var abonnement = new MenuItem
                {
                    Id = "sub-abonnement",
                    Title = "Abonnement",
                    ParentId = commercial.Id,
                    Type = ItemType.SubCategory,
                    Order = 4,
                    IsActive = true,
                    Description = "Contrats et parcs d'abonnés"
                };
                _items.Add(abonnement);
                _items.Add(new MenuItem { Id = "rep-abo-a", Title = "Rapport A", ParentId = abonnement.Id, Type = ItemType.Report, Order = 1, Engine = ReportEngine.PowerBI, Description = "Statistiques sur les nouvelles souscriptions" });
                _items.Add(new MenuItem { Id = "rep-abo-b", Title = "Rapport B", ParentId = abonnement.Id, Type = ItemType.Report, Order = 2, Engine = ReportEngine.PowerBI, Description = "Taux de résiliation et réabonnements" });

                // 1.5 Suivi des compteurs
                var compteurs = new MenuItem
                {
                    Id = "sub-compteurs",
                    Title = "Suivi des compteurs",
                    ParentId = commercial.Id,
                    Type = ItemType.SubCategory,
                    Order = 5,
                    IsActive = true,
                    Description = "Parc de compteurs et relèves"
                };
                _items.Add(compteurs);
                _items.Add(new MenuItem { Id = "rep-cpt-a", Title = "Rapport A", ParentId = compteurs.Id, Type = ItemType.Report, Order = 1, Engine = ReportEngine.PowerBI, Description = "Anomalies d'index et relèves bloquées" });
                _items.Add(new MenuItem { Id = "rep-cpt-b", Title = "Rapport B", ParentId = compteurs.Id, Type = ItemType.Report, Order = 2, Engine = ReportEngine.PowerBI, Description = "Campagnes de remplacement de compteurs" });

                // 1.6 Branchement
                var branchement = new MenuItem
                {
                    Id = "sub-branchement",
                    Title = "Branchement",
                    ParentId = commercial.Id,
                    Type = ItemType.SubCategory,
                    Order = 6,
                    IsActive = true,
                    Description = "Raccordements au réseau électrique"
                };
                _items.Add(branchement);
                _items.Add(new MenuItem { Id = "rep-bra-a", Title = "Rapport A", ParentId = branchement.Id, Type = ItemType.Report, Order = 1, Engine = ReportEngine.PowerBI, Description = "Délais de raccordement des nouveaux abonnés" });
                _items.Add(new MenuItem { Id = "rep-bra-b", Title = "Rapport B", ParentId = branchement.Id, Type = ItemType.Report, Order = 2, Engine = ReportEngine.PowerBI, Description = "Devis et travaux de branchement exécutés" });

                // 2. Finance (Level 1)
                var finance = new MenuItem
                {
                    Id = "menu-finance",
                    Title = "Finance",
                    Type = ItemType.Category,
                    Order = 2,
                    IsActive = true,
                    Description = "Direction Financière et Comptable"
                };
                _items.Add(finance);

                // 2.1 Trésorerie
                var tresorerie = new MenuItem
                {
                    Id = "sub-tresorerie",
                    Title = "Trésorerie",
                    ParentId = finance.Id,
                    Type = ItemType.SubCategory,
                    Order = 1,
                    IsActive = true,
                    Description = "Flux de trésorerie et comptes bancaires"
                };
                _items.Add(tresorerie);
                _items.Add(new MenuItem { Id = "rep-tre-a", Title = "Rapport A", ParentId = tresorerie.Id, Type = ItemType.Report, Order = 1, Engine = ReportEngine.PowerBI, Description = "Position quotidienne de trésorerie" });

                // 2.2 Budget
                var budget = new MenuItem
                {
                    Id = "sub-budget",
                    Title = "Budget",
                    ParentId = finance.Id,
                    Type = ItemType.SubCategory,
                    Order = 2,
                    IsActive = true,
                    Description = "Suivi budgétaire et engagements"
                };
                _items.Add(budget);
                _items.Add(new MenuItem { Id = "rep-bud-a", Title = "Rapport A", ParentId = budget.Id, Type = ItemType.Report, Order = 1, Engine = ReportEngine.PowerBI, Description = "Exécution budgétaire par département" });
            }
        }

        public List<MenuItem> GetMenuTree()
        {
            lock (_lock)
            {
                var roots = _items
                    .Where(i => string.IsNullOrEmpty(i.ParentId))
                    .OrderBy(i => i.Order)
                    .Select(CloneItem)
                    .ToList();

                foreach (var root in roots)
                {
                    PopulateChildren(root);
                }

                return roots;
            }
        }

        private void PopulateChildren(MenuItem parent)
        {
            var children = _items
                .Where(i => i.ParentId == parent.Id)
                .OrderBy(i => i.Order)
                .Select(CloneItem)
                .ToList();

            parent.Children = children;

            foreach (var child in children)
            {
                PopulateChildren(child);
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
                Code = source.Code,
                CreatedAt = source.CreatedAt,
                UpdatedAt = source.UpdatedAt,
                Children = new List<MenuItem>()
            };
        }

        public List<MenuItem> GetAllFlatItems()
        {
            lock (_lock)
            {
                return _items.Select(CloneItem).OrderBy(i => i.Level).ThenBy(i => i.Order).ToList();
            }
        }

        public MenuItem? GetItemById(string id)
        {
            lock (_lock)
            {
                var item = _items.FirstOrDefault(i => i.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
                return item != null ? CloneItem(item) : null;
            }
        }

        public MenuItem? GetItemByTitleAndParents(string title, string? subCategoryTitle = null, string? categoryTitle = null)
        {
            lock (_lock)
            {
                var report = _items.FirstOrDefault(i => i.Type == ItemType.Report && i.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
                if (report != null && !string.IsNullOrEmpty(subCategoryTitle))
                {
                    var parent = _items.FirstOrDefault(i => i.Id == report.ParentId);
                    if (parent != null && parent.Title.Equals(subCategoryTitle, StringComparison.OrdinalIgnoreCase))
                    {
                        return CloneItem(report);
                    }
                }
                return report != null ? CloneItem(report) : null;
            }
        }

        public void AddItem(MenuItem item)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(item.Id))
                {
                    item.Id = Guid.NewGuid().ToString("N")[..8];
                }
                item.CreatedAt = DateTime.UtcNow;
                item.UpdatedAt = DateTime.UtcNow;
                _items.Add(CloneItem(item));
            }
        }

        public bool UpdateItem(MenuItem item)
        {
            lock (_lock)
            {
                var existing = _items.FirstOrDefault(i => i.Id == item.Id);
                if (existing == null) return false;

                existing.Title = item.Title;
                existing.ParentId = item.ParentId;
                existing.Type = item.Type;
                existing.Order = item.Order;
                existing.IsActive = item.IsActive;
                existing.Engine = item.Engine;
                existing.Description = item.Description;
                existing.Code = item.Code;
                existing.UpdatedAt = DateTime.UtcNow;
                return true;
            }
        }

        public bool DeleteItem(string id)
        {
            lock (_lock)
            {
                var item = _items.FirstOrDefault(i => i.Id == id);
                if (item == null) return false;

                // Recursively delete children
                var toDelete = new List<string> { id };
                FindDescendantIds(id, toDelete);

                _items.RemoveAll(i => toDelete.Contains(i.Id));
                return true;
            }
        }

        private void FindDescendantIds(string parentId, List<string> list)
        {
            var children = _items.Where(i => i.ParentId == parentId).Select(i => i.Id).ToList();
            foreach (var childId in children)
            {
                list.Add(childId);
                FindDescendantIds(childId, list);
            }
        }

        public List<MenuItem> SearchItems(string query)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(query))
                    return new List<MenuItem>();

                return _items
                    .Where(i => i.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                (!string.IsNullOrEmpty(i.Description) && i.Description.Contains(query, StringComparison.OrdinalIgnoreCase)))
                    .Select(CloneItem)
                    .ToList();
            }
        }

        public List<string> GetBreadcrumbPath(string itemId)
        {
            lock (_lock)
            {
                var path = new List<string>();
                var current = _items.FirstOrDefault(i => i.Id == itemId);
                while (current != null)
                {
                    path.Insert(0, current.Title);
                    current = !string.IsNullOrEmpty(current.ParentId)
                        ? _items.FirstOrDefault(i => i.Id == current.ParentId)
                        : null;
                }
                return path;
            }
        }

        public void ResetToDefault()
        {
            InitializeDefaultData();
        }
    }
}
