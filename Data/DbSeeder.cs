using Microsoft.EntityFrameworkCore;
using PortailSocadel.Models;

namespace PortailSocadel.Data
{
    public static class DbSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

            // Create database automatically
            context.Database.EnsureCreated();

            // Seed Users if empty
            if (!context.Users.Any())
            {
                using var sha256 = System.Security.Cryptography.SHA256.Create();
                var adminPasswordHash = Convert.ToBase64String(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes("Admin123!")));
                var userPasswordHash = Convert.ToBase64String(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes("User123!")));

                context.Users.AddRange(
                    new User
                    {
                        Id = "usr-admin-01",
                        Email = "admin@socadel.cm",
                        FullName = "Administrateur SOCADEL",
                        PasswordHash = adminPasswordHash,
                        Role = "Admin",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Id = "usr-user-01",
                        Email = "user@socadel.cm",
                        FullName = "Naomi TSAGUE",
                        PasswordHash = userPasswordHash,
                        Role = "User",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                );
                context.SaveChanges();
            }

            // Look for any menu items.
            if (context.MenuItems.Any())
            {
                return;   // DB has been seeded
            }

            var items = new List<MenuItem>();

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
                Description = "Direction Commerciale et Gestion de la Clientèle",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(commercial);

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
                Description = "Suivi des encaissements, caisses et modes de paiement",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(encaissement);
            items.Add(new MenuItem 
            { 
                Id = "rep-enc-synth", 
                Title = "Synthèse journalière des encaissements", 
                ParentId = encaissement.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-ENC-01",
                ReportUrl = "https://app.powerbi.com/view?r=eyJrIjoiM2ZmZGNmMjctZmU3OC00MzdjLTgyN2EtZWMzZWM4NTM1NjYwIiwidCI6IjY5OWFjZTY3LWQyZTQtNGJjZC1iMzAzLWQyYmJlMmI5YmJmMSJ9",
                Description = "Suivi quotidien des flux de caisse, dépôts bancaires et ventilations par agence",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            items.Add(new MenuItem 
            { 
                Id = "rep-enc-modes", 
                Title = "Répartition par mode de règlement", 
                ParentId = encaissement.Id, 
                Type = ItemType.Report, 
                Order = 2, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-ENC-02",
                ReportUrl = "https://app.powerbi.com/view?r=eyJrIjoiMTY5ZTIyN2MtOGVmYy00NTc3LTkzMWMtZmNkMTZiNDc4NWJkIiwidCI6ImZjZTBkOTIyLWMzMjktNGMwMC04MTY3LTZkYzQ4ZTM3ZWEwNSJ9",
                Description = "Analyse des règlements par espèces, virements, cartes et mobile money",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            // 1.1.1 RELEVES (Sous-menu dans Encaissement)
            var releves = new MenuItem
            {
                Id = "sub-releves",
                Title = "RELEVES",
                ParentId = encaissement.Id,
                Type = ItemType.SubCategory,
                Order = 3,
                IsActive = true,
                Code = "REL",
                Description = "Sous-menu RELEVES sous Encaissement",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(releves);
            items.Add(new MenuItem 
            { 
                Id = "rep-rel-synth", 
                Title = "Synthèse des relevés d'index", 
                ParentId = releves.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-REL-01",
                Description = "Rapport de synthèse sur les relevés d'index de consommation",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            // 1.1.1.1 Relevés Spéciaux (Sous-sous-menu dans RELEVES)
            var relevesSpeciaux = new MenuItem
            {
                Id = "sub-releves-speciaux",
                Title = "Relevés Spéciaux",
                ParentId = releves.Id,
                Type = ItemType.SubCategory,
                Order = 2,
                IsActive = true,
                Code = "REL-SPEC",
                Description = "Sous-sous-menu Relevés Spéciaux sous RELEVES",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(relevesSpeciaux);
            items.Add(new MenuItem 
            { 
                Id = "rep-rel-spec-01", 
                Title = "Suivi des relèves haute tension & industrielles", 
                ParentId = relevesSpeciaux.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-REL-SPEC-01",
                Description = "Rapport spécialisé sur les relèves des compteurs industriels",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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
                Description = "Gestion des émissions de factures et volumes d'énergie",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(facturation);
            items.Add(new MenuItem 
            { 
                Id = "rep-fac-mensuel", 
                Title = "Suivi mensuel de facturation", 
                ParentId = facturation.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-FAC-01",
                Description = "Volumes d'énergie facturés, montants HT/TTC et analyse comparative M-1",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            items.Add(new MenuItem 
            { 
                Id = "rep-fac-grands-comptes", 
                Title = "Portefeuille grands comptes & industriels", 
                ParentId = facturation.Id, 
                Type = ItemType.Report, 
                Order = 2, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-FAC-02",
                Description = "Facturation haute tension et suivi des gros consommateurs",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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
                Description = "Suivi des impayés, balance âgée et contentieux",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(recouvrement);
            items.Add(new MenuItem 
            { 
                Id = "rep-rec-taux", 
                Title = "Taux de recouvrement & balance âgée", 
                ParentId = recouvrement.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-REC-01",
                Description = "Tableau de bord de recouvrement commercial et balance âgée des créances",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            items.Add(new MenuItem 
            { 
                Id = "rep-rec-contentieux", 
                Title = "Dossiers en contentieux & relances", 
                ParentId = recouvrement.Id, 
                Type = ItemType.Report, 
                Order = 2, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-REC-02",
                Description = "Suivi des créances contentieuses et actions juridiques",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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
                Description = "Contrats et parcs d'abonnés",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(abonnement);
            items.Add(new MenuItem 
            { 
                Id = "rep-abo-parc", 
                Title = "Parc abonnés & nouvelles souscriptions", 
                ParentId = abonnement.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-ABO-01",
                Description = "Statistiques sur les souscriptions actives par catégorie tarifaire",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            items.Add(new MenuItem 
            { 
                Id = "rep-abo-resiliations", 
                Title = "Taux de résiliation & réabonnements", 
                ParentId = abonnement.Id, 
                Type = ItemType.Report, 
                Order = 2, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-ABO-02",
                Description = "Analyse des motifs de résiliation et réactivations",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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
                Description = "Parc de compteurs, relèves et détection d'anomalies",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(compteurs);
            items.Add(new MenuItem 
            { 
                Id = "rep-cpt-parc", 
                Title = "Inventaire du parc compteurs", 
                ParentId = compteurs.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-CPT-01",
                Description = "Statut du parc de compteurs électromécaniques, électroniques et communicants",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            items.Add(new MenuItem 
            { 
                Id = "rep-cpt-anomalies", 
                Title = "Anomalies de relève & compteurs bloqués", 
                ParentId = compteurs.Id, 
                Type = ItemType.Report, 
                Order = 2, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-CPT-02",
                SimulateError = true, 
                Description = "Détection des index incohérents, compteurs bloqués et tentatives de fraude",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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
                Description = "Raccordements au réseau électrique",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(branchement);
            items.Add(new MenuItem 
            { 
                Id = "rep-bra-delais", 
                Title = "Délais d'instruction des raccordements", 
                ParentId = branchement.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-BRA-01",
                Description = "Temps moyen d'instruction des demandes et délais de raccordement",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            items.Add(new MenuItem 
            { 
                Id = "rep-bra-travaux", 
                Title = "Suivi des chantiers & travaux réseau", 
                ParentId = branchement.Id, 
                Type = ItemType.Report, 
                Order = 2, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-BRA-02",
                Description = "Avancement des devis et travaux de branchement exécutés",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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
                Description = "Direction Financière et Comptable",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(finance);

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
                Description = "Flux de trésorerie et comptes bancaires",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(tresorerie);
            items.Add(new MenuItem 
            { 
                Id = "rep-tre-position", 
                Title = "Position quotidienne de trésorerie", 
                ParentId = tresorerie.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-TRE-01",
                Description = "Solde consolidé des comptes bancaires, encaissements et décaissements du jour",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            items.Add(new MenuItem 
            { 
                Id = "rep-tre-previsions", 
                Title = "Prévisions des flux à court terme", 
                ParentId = tresorerie.Id, 
                Type = ItemType.Report, 
                Order = 2, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-TRE-02",
                Description = "Modélisation des encaissements et décaissements prévisionnels à 30 jours",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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
                Description = "Suivi budgétaire et engagements par direction",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(budget);
            items.Add(new MenuItem 
            { 
                Id = "rep-bud-execution", 
                Title = "Exécution budgétaire par direction", 
                ParentId = budget.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-BUD-01",
                Description = "Taux de consommation des crédits budgétaires OPEX et CAPEX",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            items.Add(new MenuItem 
            { 
                Id = "rep-bud-ecarts", 
                Title = "Analyse des écarts budgétaires", 
                ParentId = budget.Id, 
                Type = ItemType.Report, 
                Order = 2, 
                Engine = ReportEngine.PowerBI, 
                Code = "REP-BUD-02",
                Description = "Contrôle budgétaire analytique et alertes de dépassement de lignes",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            context.MenuItems.AddRange(items);
            context.SaveChanges();
        }
    }
}
