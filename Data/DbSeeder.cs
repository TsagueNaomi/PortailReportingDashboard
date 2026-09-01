using Microsoft.EntityFrameworkCore;
using PortailSocadel.Models;

namespace PortailSocadel.Data
{
    public static class DbSeeder
    {
        private static readonly string DataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "portailsocadel_store.json");

        public class AppDataDto
        {
            public List<MenuItem> MenuItems { get; set; } = new();
            public List<User> Users { get; set; } = new();
        }

        public static void SaveData(AppDbContext context)
        {
            try
            {
                var dto = new AppDataDto
                {
                    MenuItems = context.MenuItems.AsNoTracking().ToList(),
                    Users = context.Users.AsNoTracking().ToList()
                };
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                var json = System.Text.Json.JsonSerializer.Serialize(dto, options);
                File.WriteAllText(DataFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PERSISTENCE SAVE ERROR] {ex.Message}");
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                context.Database.EnsureCreated();
            }
            catch { }

            // 1. Restore Users if missing
            if (File.Exists(DataFilePath) && !context.Users.Any())
            {
                try
                {
                    var json = File.ReadAllText(DataFilePath);
                    var dto = System.Text.Json.JsonSerializer.Deserialize<AppDataDto>(json);
                    if (dto?.Users != null && dto.Users.Any())
                    {
                        context.Users.AddRange(dto.Users);
                        context.SaveChanges();
                    }
                }
                catch { }
            }

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
            if (!context.MenuItems.Any())
            {
                if (File.Exists(DataFilePath))
                {
                    try
                    {
                        var json = File.ReadAllText(DataFilePath);
                        var dto = System.Text.Json.JsonSerializer.Deserialize<AppDataDto>(json);
                        if (dto?.MenuItems != null && dto.MenuItems.Any())
                        {
                            context.MenuItems.AddRange(dto.MenuItems);
                            context.SaveChanges();
                            return;
                        }
                    }
                    catch { }
                }

                SeedMenuItems(context);
                SaveData(context);
            }
            else
            {
                // Ensure store backup is always up to date
                SaveData(context);
            }
        }

        public static void ReSeedDefaults(AppDbContext context)
        {
            if (context.MenuItems.Any())
            {
                context.MenuItems.RemoveRange(context.MenuItems);
                context.SaveChanges();
            }
            SeedMenuItems(context);
        }

        public static void SeedMenuItems(AppDbContext context)
        {
            var items = new List<MenuItem>();

            // =============================================================
            // 1. COMMERCIAL (Menu)
            // =============================================================
            var commercial = new MenuItem
            {
                Id = "menu-commercial",
                Title = "Commercial",
                Type = ItemType.Category,
                Order = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(commercial);

            // 1.1 Encaissement (Sous-menu)
            var encaissement = new MenuItem
            {
                Id = "sub-encaissement",
                Title = "Encaissement",
                ParentId = commercial.Id,
                Type = ItemType.SubCategory,
                Order = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(encaissement);

            // 1.1.1 Synthèses (Sous-menu)
            var syntheses = new MenuItem
            {
                Id = "sub-syntheses",
                Title = "Synthèses",
                ParentId = encaissement.Id,
                Type = ItemType.SubCategory,
                Order = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(syntheses);

            items.Add(new MenuItem 
            { 
                Id = "rep-enc-synth", 
                Title = "Synthèse journalière des encaissements", 
                ParentId = syntheses.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.PowerBI, 
                ReportUrl = "https://app.powerbi.com/view?r=eyJrIjoiM2ZmZGNmMjctZmU3OC00MzdjLTgyN2EtZWMzZWM4NTM1NjYwIiwidCI6IjY5OWFjZTY3LWQyZTQtNGJjZC1iMzAzLWQyYmJlMmI5YmJmMSJ9",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            items.Add(new MenuItem 
            { 
                Id = "rep-enc-modes", 
                Title = "Répartition par mode de règlement", 
                ParentId = syntheses.Id, 
                Type = ItemType.Report, 
                Order = 2, 
                Engine = ReportEngine.PowerBI, 
                ReportUrl = "https://app.powerbi.com/view?r=eyJrIjoiMTY5ZTIyN2MtOGVmYy00NTc3LTkzMWMtZmNkMTZiNDc4NWJkIiwidCI6ImZjZTBkOTIyLWMzMjktNGMwMC04MTY3LTZkYzQ4ZTM3ZWEwNSJ9",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            // 1.1.2 RELEVES (Sous-menu)
            var releves = new MenuItem
            {
                Id = "sub-releves",
                Title = "RELEVES",
                ParentId = encaissement.Id,
                Type = ItemType.SubCategory,
                Order = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(releves);

            // 1.1.2.1 Synthèses Relevés (Sous-menu)
            var synthesesReleves = new MenuItem
            {
                Id = "sub-syntheses-releves",
                Title = "Synthèses Relevés",
                ParentId = releves.Id,
                Type = ItemType.SubCategory,
                Order = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(synthesesReleves);

            items.Add(new MenuItem 
            { 
                Id = "rep-rel-synth", 
                Title = "Synthèse des relevés d'index", 
                ParentId = synthesesReleves.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.PowerBI, 
                ReportUrl = "https://app.powerbi.com/view?r=eyJrIjoiM2ZmZGNmMjctZmU3OC00MzdjLTgyN2EtZWMzZWM4NTM1NjYwIiwidCI6IjY5OWFjZTY3LWQyZTQtNGJjZC1iMzAzLWQyYmJlMmI5YmJmMSJ9",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            // 1.1.2.2 Relevés Spéciaux (Sous-menu)
            var relevesSpeciaux = new MenuItem
            {
                Id = "sub-releves-speciaux",
                Title = "Relevés Spéciaux",
                ParentId = releves.Id,
                Type = ItemType.SubCategory,
                Order = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(relevesSpeciaux);

            items.Add(new MenuItem 
            { 
                Id = "rep-rel-spec-01", 
                Title = "Suivi des relevés haute tension & industrielles", 
                ParentId = relevesSpeciaux.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.SSRS, 
                ReportUrl = "http://ssrs.socadel.cm/Reports/Pages/Report.aspx?ItemPath=/Commercial/RelevesHT",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            // =============================================================
            // 2. FACTURATION (Menu)
            // =============================================================
            var facturation = new MenuItem
            {
                Id = "menu-facturation",
                Title = "Facturation",
                Type = ItemType.Category,
                Order = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(facturation);

            // 2.1 Suivis (Sous-menu)
            var suivis = new MenuItem
            {
                Id = "sub-suivis",
                Title = "Suivis",
                ParentId = facturation.Id,
                Type = ItemType.SubCategory,
                Order = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            items.Add(suivis);

            items.Add(new MenuItem 
            { 
                Id = "rep-fac-mensuel", 
                Title = "Suivi mensuel de facturation", 
                ParentId = suivis.Id, 
                Type = ItemType.Report, 
                Order = 1, 
                Engine = ReportEngine.PowerBI, 
                ReportUrl = "https://app.powerbi.com/view?r=eyJrIjoiMTY5ZTIyN2MtOGVmYy00NTc3LTkzMWMtZmNkMTZiNDc4NWJkIiwidCI6ImZjZTBkOTIyLWMzMjktNGMwMC04MTY3LTZkYzQ4ZTM3ZWEwNSJ9",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            items.Add(new MenuItem 
            { 
                Id = "rep-fac-grands-comptes", 
                Title = "Portefeuille grands comptes & industriels", 
                Type = ItemType.Report, 
                Order = 2, 
                Engine = ReportEngine.PowerBI, 
                ReportUrl = "https://app.powerbi.com/view?r=eyJrIjoiM2ZmZGNmMjctZmU3OC00MzdjLTgyN2EtZWMzZWM4NTM1NjYwIiwidCI6IjY5OWFjZTY3LWQyZTQtNGJjZC1iMzAzLWQyYmJlMmI5YmJmMSJ9",
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
                ReportUrl = "https://app.powerbi.com/view?r=eyJrIjoiMTY5ZTIyN2MtOGVmYy00NTc3LTkzMWMtZmNkMTZiNDc4NWJkIiwidCI6ImZjZTBkOTIyLWMzMjktNGMwMC04MTY3LTZkYzQ4ZTM3ZWEwNSJ9",
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
                ReportUrl = "https://app.powerbi.com/view?r=eyJrIjoiM2ZmZGNmMjctZmU3OC00MzdjLTgyN2EtZWMzZWM4NTM1NjYwIiwidCI6IjY5OWFjZTY3LWQyZTQtNGJjZC1iMzAzLWQyYmJlMmI5YmJmMSJ9",
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
                Engine = ReportEngine.SSRS, 
                ReportUrl = "http://ssrs.socadel.cm/Reports/Pages/Report.aspx?ItemPath=/Finance/ExecBudgetaire",
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
                Engine = ReportEngine.SSRS, 
                ReportUrl = "http://ssrs.socadel.cm/Reports/Pages/Report.aspx?ItemPath=/Finance/EcartsBudgetaires",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            context.MenuItems.AddRange(items);
            context.SaveChanges();
            SaveData(context);
        }
    }
}
