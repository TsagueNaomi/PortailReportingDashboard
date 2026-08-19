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

                // =============================================================
                // 1. DIRECTION COMMERCIALE (Level 1)
                // =============================================================
                var commercial = new MenuItem
                {
                    Id = "menu-commercial",
                    Title = "Commercial",
                    Type = ItemType.Category,
                    Order = 1,
                    IsActive = true,
                    Code = "DIR-COMM",
                    Description = "Direction Commerciale et Gestion de la Clientèle"
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
                    Code = "ENC",
                    Description = "Suivi des encaissements, caisses et modes de paiement"
                };
                _items.Add(encaissement);
                _items.Add(new MenuItem 
                { 
                    Id = "rep-enc-synth", 
                    Title = "Synthèse journalière des encaissements", 
                    ParentId = encaissement.Id, 
                    Type = ItemType.Report, 
                    Order = 1, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-ENC-01",
                    ReportUrl = "https://app.powerbi.com/view?r=eyJrIjoiM2ZmZGNmMjctZmU3OC00MzdjLTgyN2EtZWMzZWM4NTM1NjYwIiwidCI6IjY5OWFjZTY3LWQyZTQtNGJjZC1iMzAzLWQyYmJlMmI5YmJmMSJ9",
                    Description = "Suivi quotidien des flux de caisse, dépôts bancaires et ventilations par agence" 
                });
                _items.Add(new MenuItem 
                { 
                    Id = "rep-enc-modes", 
                    Title = "Répartition par mode de règlement", 
                    ParentId = encaissement.Id, 
                    Type = ItemType.Report, 
                    Order = 2, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-ENC-02",
                    ReportUrl = "https://app.powerbi.com/view?r=eyJrIjoiMTY5ZTIyN2MtOGVmYy00NTc3LTkzMWMtZmNkMTZiNDc4NWJkIiwidCI6ImZjZTBkOTIyLWMzMjktNGMwMC04MTY3LTZkYzQ4ZTM3ZWEwNSJ9",
                    Description = "Analyse des règlements par espèces, virements, cartes et mobile money" 
                });

                // 1.2 Facturation
                var facturation = new MenuItem
                {
                    Id = "sub-facturation",
                    Title = "Facturation",
                    ParentId = commercial.Id,
                    Type = ItemType.SubCategory,
                    Order = 2,
                    IsActive = true,
                    Code = "FAC",
                    Description = "Gestion des émissions de factures et volumes d'énergie"
                };
                _items.Add(facturation);
                _items.Add(new MenuItem 
                { 
                    Id = "rep-fac-mensuel", 
                    Title = "Suivi mensuel de facturation", 
                    ParentId = facturation.Id, 
                    Type = ItemType.Report, 
                    Order = 1, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-FAC-01",
                    Description = "Volumes d'énergie facturés, montants HT/TTC et analyse comparative M-1" 
                });
                _items.Add(new MenuItem 
                { 
                    Id = "rep-fac-grands-comptes", 
                    Title = "Portefeuille grands comptes & industriels", 
                    ParentId = facturation.Id, 
                    Type = ItemType.Report, 
                    Order = 2, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-FAC-02",
                    Description = "Facturation haute tension et suivi des gros consommateurs" 
                });

                // 1.3 Recouvrement
                var recouvrement = new MenuItem
                {
                    Id = "sub-recouvrement",
                    Title = "Recouvrement",
                    ParentId = commercial.Id,
                    Type = ItemType.SubCategory,
                    Order = 3,
                    IsActive = true,
                    Code = "REC",
                    Description = "Suivi des impayés, balance âgée et contentieux"
                };
                _items.Add(recouvrement);
                _items.Add(new MenuItem 
                { 
                    Id = "rep-rec-taux", 
                    Title = "Taux de recouvrement & balance âgée", 
                    ParentId = recouvrement.Id, 
                    Type = ItemType.Report, 
                    Order = 1, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-REC-01",
                    Description = "Tableau de bord de recouvrement commercial et balance âgée des créances" 
                });
                _items.Add(new MenuItem 
                { 
                    Id = "rep-rec-contentieux", 
                    Title = "Dossiers en contentieux & relances", 
                    ParentId = recouvrement.Id, 
                    Type = ItemType.Report, 
                    Order = 2, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-REC-02",
                    Description = "Suivi des créances contentieuses et actions juridiques" 
                });

                // 1.4 Abonnement
                var abonnement = new MenuItem
                {
                    Id = "sub-abonnement",
                    Title = "Abonnement",
                    ParentId = commercial.Id,
                    Type = ItemType.SubCategory,
                    Order = 4,
                    IsActive = true,
                    Code = "ABO",
                    Description = "Contrats et parcs d'abonnés"
                };
                _items.Add(abonnement);
                _items.Add(new MenuItem 
                { 
                    Id = "rep-abo-parc", 
                    Title = "Parc abonnés & nouvelles souscriptions", 
                    ParentId = abonnement.Id, 
                    Type = ItemType.Report, 
                    Order = 1, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-ABO-01",
                    Description = "Statistiques sur les souscriptions actives par catégorie tarifaire" 
                });
                _items.Add(new MenuItem 
                { 
                    Id = "rep-abo-resiliations", 
                    Title = "Taux de résiliation & réabonnements", 
                    ParentId = abonnement.Id, 
                    Type = ItemType.Report, 
                    Order = 2, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-ABO-02",
                    Description = "Analyse des motifs de résiliation et réactivations" 
                });

                // 1.5 Suivi des compteurs
                var compteurs = new MenuItem
                {
                    Id = "sub-compteurs",
                    Title = "Suivi des compteurs",
                    ParentId = commercial.Id,
                    Type = ItemType.SubCategory,
                    Order = 5,
                    IsActive = true,
                    Code = "CPT",
                    Description = "Parc de compteurs, relèves et détection d'anomalies"
                };
                _items.Add(compteurs);
                _items.Add(new MenuItem 
                { 
                    Id = "rep-cpt-parc", 
                    Title = "Inventaire du parc compteurs", 
                    ParentId = compteurs.Id, 
                    Type = ItemType.Report, 
                    Order = 1, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-CPT-01",
                    Description = "Statut du parc de compteurs électromécaniques, électroniques et communicants" 
                });
                // Simulated Error Report for realistic demonstration directly in the navigation menu (Point 3)
                _items.Add(new MenuItem 
                { 
                    Id = "rep-cpt-anomalies", 
                    Title = "Anomalies de relève & compteurs bloqués", 
                    ParentId = compteurs.Id, 
                    Type = ItemType.Report, 
                    Order = 2, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-CPT-02",
                    SimulateError = true, // Triggers error state in realistic navigation
                    Description = "Détection des index incohérents, compteurs bloqués et tentatives de fraude" 
                });

                // 1.6 Branchement
                var branchement = new MenuItem
                {
                    Id = "sub-branchement",
                    Title = "Branchement",
                    ParentId = commercial.Id,
                    Type = ItemType.SubCategory,
                    Order = 6,
                    IsActive = true,
                    Code = "BRA",
                    Description = "Raccordements au réseau électrique"
                };
                _items.Add(branchement);
                _items.Add(new MenuItem 
                { 
                    Id = "rep-bra-delais", 
                    Title = "Délais d'instruction des raccordements", 
                    ParentId = branchement.Id, 
                    Type = ItemType.Report, 
                    Order = 1, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-BRA-01",
                    Description = "Temps moyen d'instruction des demandes et délais de raccordement" 
                });
                _items.Add(new MenuItem 
                { 
                    Id = "rep-bra-travaux", 
                    Title = "Suivi des chantiers & travaux réseau", 
                    ParentId = branchement.Id, 
                    Type = ItemType.Report, 
                    Order = 2, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-BRA-02",
                    Description = "Avancement des devis et travaux de branchement exécutés" 
                });

                // =============================================================
                // 2. DIRECTION FINANCIÈRE (Level 1)
                // =============================================================
                var finance = new MenuItem
                {
                    Id = "menu-finance",
                    Title = "Finance",
                    Type = ItemType.Category,
                    Order = 2,
                    IsActive = true,
                    Code = "DIR-FIN",
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
                    Code = "TRE",
                    Description = "Flux de trésorerie et comptes bancaires"
                };
                _items.Add(tresorerie);
                _items.Add(new MenuItem 
                { 
                    Id = "rep-tre-position", 
                    Title = "Position quotidienne de trésorerie", 
                    ParentId = tresorerie.Id, 
                    Type = ItemType.Report, 
                    Order = 1, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-TRE-01",
                    Description = "Solde consolidé des comptes bancaires, encaissements et décaissements du jour" 
                });
                _items.Add(new MenuItem 
                { 
                    Id = "rep-tre-previsions", 
                    Title = "Prévisions des flux à court terme", 
                    ParentId = tresorerie.Id, 
                    Type = ItemType.Report, 
                    Order = 2, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-TRE-02",
                    Description = "Modélisation des encaissements et décaissements prévisionnels à 30 jours" 
                });

                // 2.2 Budget
                var budget = new MenuItem
                {
                    Id = "sub-budget",
                    Title = "Budget",
                    ParentId = finance.Id,
                    Type = ItemType.SubCategory,
                    Order = 2,
                    IsActive = true,
                    Code = "BUD",
                    Description = "Suivi budgétaire et engagements par direction"
                };
                _items.Add(budget);
                _items.Add(new MenuItem 
                { 
                    Id = "rep-bud-execution", 
                    Title = "Exécution budgétaire par direction", 
                    ParentId = budget.Id, 
                    Type = ItemType.Report, 
                    Order = 1, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-BUD-01",
                    Description = "Taux de consommation des crédits budgétaires OPEX et CAPEX" 
                });
                _items.Add(new MenuItem 
                { 
                    Id = "rep-bud-ecarts", 
                    Title = "Analyse des écarts budgétaires", 
                    ParentId = budget.Id, 
                    Type = ItemType.Report, 
                    Order = 2, 
                    Engine = ReportEngine.PowerBI, 
                    Code = "REP-BUD-02",
                    Description = "Contrôle budgétaire analytique et alertes de dépassement de lignes" 
                });
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

                // If adding a Level 1 Category, ensure order goes to the bottom of the list
                if (item.Type == ItemType.Category)
                {
                    var maxCatOrder = _items.Where(i => i.Type == ItemType.Category).Select(i => i.Order).DefaultIfEmpty(0).Max();
                    item.Order = maxCatOrder + 1;
                }
                else if (item.Type == ItemType.SubCategory)
                {
                    var maxSubOrder = _items.Where(i => i.Type == ItemType.SubCategory && i.ParentId == item.ParentId).Select(i => i.Order).DefaultIfEmpty(0).Max();
                    item.Order = maxSubOrder + 1;
                }
                else if (item.Type == ItemType.Report)
                {
                    var maxRepOrder = _items.Where(i => i.Type == ItemType.Report && i.ParentId == item.ParentId).Select(i => i.Order).DefaultIfEmpty(0).Max();
                    item.Order = maxRepOrder + 1;
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
                existing.ReportUrl = item.ReportUrl;
                existing.Code = item.Code;
                existing.SimulateError = item.SimulateError;
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
                                (!string.IsNullOrEmpty(i.Description) && i.Description.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                                (!string.IsNullOrEmpty(i.Code) && i.Code.Contains(query, StringComparison.OrdinalIgnoreCase)))
                    .Select(CloneItem)
                    .ToList();
            }
        }

        public List<string> GetBreadcrumbPath(string itemId)
        {
            return GetBreadcrumbNodes(itemId).Select(i => i.Title).ToList();
        }

        public List<MenuItem> GetBreadcrumbNodes(string itemId)
        {
            lock (_lock)
            {
                var list = new List<MenuItem>();
                var current = _items.FirstOrDefault(i => i.Id == itemId);
                while (current != null)
                {
                    list.Insert(0, CloneItem(current));
                    current = !string.IsNullOrEmpty(current.ParentId)
                        ? _items.FirstOrDefault(i => i.Id == current.ParentId)
                        : null;
                }
                return list;
            }
        }

        public void ResetToDefault()
        {
            InitializeDefaultData();
        }
    }
}
